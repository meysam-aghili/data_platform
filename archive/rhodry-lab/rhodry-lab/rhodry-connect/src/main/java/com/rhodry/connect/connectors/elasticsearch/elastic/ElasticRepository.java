/**
 * Copyright © 2018 Dario Balinzo (dariobalinzo@gmail.com)
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.rhodry.connect.connectors.elasticsearch.elastic;

import com.rhodry.connect.connectors.elasticsearch.elastic.response.Cursor;
import com.rhodry.connect.connectors.elasticsearch.elastic.response.PageResult;
import org.elasticsearch.action.search.SearchRequest;
import org.elasticsearch.action.search.SearchResponse;
import org.elasticsearch.client.Request;
import org.elasticsearch.client.RequestOptions;
import org.elasticsearch.client.Response;
import org.elasticsearch.index.query.QueryBuilder;
import org.elasticsearch.search.builder.SearchSourceBuilder;
import org.elasticsearch.search.sort.SortOrder;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.*;
import java.util.stream.Collectors;
import java.time.ZonedDateTime;

import static org.elasticsearch.index.query.QueryBuilders.*;

public final class ElasticRepository {
    private final static Logger logger = LoggerFactory.getLogger(ElasticRepository.class);

    private final ElasticConnection elasticConnection;

    private final String cursorSearchField;
    private final String secondaryCursorSearchField;
    private final CursorField cursorField;
    private final CursorField secondaryCursorField;

    private int pageSize = 5000;
    private int delayMs = -1;
    

    public ElasticRepository(ElasticConnection elasticConnection) {
        this(elasticConnection, "_id");
    }

    public ElasticRepository(ElasticConnection elasticConnection, String cursorField) {
        this(elasticConnection, cursorField, null);
    }

    public ElasticRepository(ElasticConnection elasticConnection, String cursorSearchField, String secondaryCursorSearchField) {
        this.elasticConnection = elasticConnection;
        this.cursorSearchField = cursorSearchField;
        this.cursorField = new CursorField(cursorSearchField);
        this.secondaryCursorSearchField = secondaryCursorSearchField;
        this.secondaryCursorField = secondaryCursorSearchField == null ? null : new CursorField(secondaryCursorSearchField);
    }

    private String getQueryUpperBound(String index) throws IOException, InterruptedException {
        SearchSourceBuilder lastDocSearchSourceBuilder = new SearchSourceBuilder()
            .query(matchAllQuery())
            .sort(cursorSearchField, SortOrder.DESC)
            .size(1);
        SearchRequest lastDocSearchRequest = new SearchRequest(index)
            .source(lastDocSearchSourceBuilder);
        SearchResponse lastDocSearchResponse = executeSearch(lastDocSearchRequest);
        Map<String, Object> lastIndexedDoc = extractDocuments(lastDocSearchResponse).get(0);
        String lastIndexedTimestamp = cursorField.read(lastIndexedDoc);
        String upperBound = ZonedDateTime
            .parse(lastIndexedTimestamp)
            .minusSeconds(delayMs)
            .toString();
        return upperBound;
    }

    public PageResult searchAfter(String index, Cursor cursor) throws IOException, InterruptedException {
        String upperBound = null;
        if (delayMs > 0)
        {
            try {
                upperBound = getQueryUpperBound(index);
                logger.info("Fetching documents up to " + upperBound);
            }
            catch (Exception e) {
                logger.info("Could not figure out the delay. Fetching to the last available document...");
            }
        }

        QueryBuilder queryBuilder;
        if (!Objects.equals(upperBound, null)) {
            queryBuilder = cursor.getPrimaryCursor() == null ?
                matchAllQuery() :
                buildInBetween(cursorSearchField, cursor.getPrimaryCursor(), upperBound);
        } else {
            queryBuilder = cursor.getPrimaryCursor() == null ?
                matchAllQuery() :
                buildGreaterThan(cursorSearchField, cursor.getPrimaryCursor());
        }

        SearchSourceBuilder searchSourceBuilder = new SearchSourceBuilder()
                .query(queryBuilder)
                .size(pageSize)
                .sort(cursorSearchField, SortOrder.ASC);

        SearchRequest searchRequest = new SearchRequest(index)
                .source(searchSourceBuilder);

        SearchResponse response = executeSearch(searchRequest);

        List<Map<String, Object>> documents = extractDocuments(response);

        Cursor lastCursor;
        if (documents.isEmpty()) {
            lastCursor = Cursor.empty();
        } else {
            Map<String, Object> lastDocument = documents.get(documents.size() - 1);
            lastCursor = new Cursor(cursorField.read(lastDocument));
        }
        return new PageResult(index, documents, lastCursor);
    }

    private List<Map<String, Object>> extractDocuments(SearchResponse response) {
        return Arrays.stream(response.getHits().getHits())
                .map(hit -> {
                    Map<String, Object> sourceMap = hit.getSourceAsMap();
                    sourceMap.put("es-id", hit.getId());
                    sourceMap.put("es-index", hit.getIndex());
                    return sourceMap;
                }).collect(Collectors.toList());
    }

    public PageResult searchAfterWithSecondarySort(String index, Cursor cursor) throws IOException, InterruptedException {
        Objects.requireNonNull(secondaryCursorField);
        String primaryCursor = cursor.getPrimaryCursor();
        String secondaryCursor = cursor.getSecondaryCursor();
        boolean noPrevCursor = primaryCursor == null && secondaryCursor == null;

        QueryBuilder queryBuilder = noPrevCursor ? matchAllQuery() :
                getSecondarySortFieldQuery(primaryCursor, secondaryCursor);

        SearchSourceBuilder searchSourceBuilder = new SearchSourceBuilder()
                .query(queryBuilder)
                .size(pageSize)
                .sort(cursorSearchField, SortOrder.ASC)
                .sort(secondaryCursorSearchField, SortOrder.ASC);

        SearchRequest searchRequest = new SearchRequest(index)
                .source(searchSourceBuilder);

        SearchResponse response = executeSearch(searchRequest);

        List<Map<String, Object>> documents = extractDocuments(response);

        Cursor lastCursor;
        if (documents.isEmpty()) {
            lastCursor = Cursor.empty();
        } else {
            Map<String, Object> lastDocument = documents.get(documents.size() - 1);
            String primaryCursorValue = cursorField.read(lastDocument);
            String secondaryCursorValue = secondaryCursorField.read(lastDocument);
            lastCursor = new Cursor(primaryCursorValue, secondaryCursorValue);
        }
        return new PageResult(index, documents, lastCursor);
    }

    private QueryBuilder buildGreaterThan(String cursorField, String cursorValue) {
        return rangeQuery(cursorField).from(cursorValue, false);
    }

    private QueryBuilder buildInBetween(String cursorField, String cursorValue, String upperBound) {
        return rangeQuery(cursorField).from(cursorValue, false).to(upperBound, false);
    }

    private QueryBuilder getSecondarySortFieldQuery(String primaryCursor, String secondaryCursor) {
        if (secondaryCursor == null) {
            return buildGreaterThan(cursorSearchField, primaryCursor);
        }
        return boolQuery()
                .minimumShouldMatch(1)
                .should(buildGreaterThan(cursorSearchField, primaryCursor))
                .should(
                        boolQuery()
                                .filter(matchQuery(cursorSearchField, primaryCursor))
                                .filter(buildGreaterThan(secondaryCursorSearchField, secondaryCursor))
                );
    }

    private SearchResponse executeSearch(SearchRequest searchRequest) throws IOException, InterruptedException {
        int maxTrials = elasticConnection.getMaxConnectionAttempts();
        if (maxTrials <= 0) {
            throw new IllegalArgumentException("MaxConnectionAttempts should be > 0");
        }
        IOException lastError = null;
        for (int i = 0; i < maxTrials; ++i) {
            try {
                return elasticConnection.getClient()
                        .search(searchRequest, RequestOptions.DEFAULT);
            } catch (IOException e) {
                lastError = e;
                Thread.sleep(elasticConnection.getConnectionRetryBackoff());
            }
        }
        throw lastError;
    }

    public List<String> catIndices(String prefix) {
        Response resp;
        try {
            resp = elasticConnection.getClient()
                    .getLowLevelClient()
                    .performRequest(new Request("GET", "/_cat/indices"));
        } catch (IOException e) {
            logger.error("error in searching index names");
            throw new RuntimeException(e);
        }

        List<String> result = new ArrayList<>();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(resp.getEntity().getContent()))) {
            String line;

            while ((line = reader.readLine()) != null) {
                String index = line.split("\\s+")[2];
                if (index.startsWith(prefix)) {
                    result.add(index);
                }
            }
        } catch (IOException e) {
            logger.error("error while getting indices", e);
        }

        Collections.sort(result);

        return result;
    }

    public void refreshIndex(String index) {
        try {
            elasticConnection.getClient()
                    .getLowLevelClient()
                    .performRequest(new Request("POST", "/" + index + "/_refresh"));
        } catch (IOException e) {
            logger.error("error in refreshing index " + index);
            throw new RuntimeException(e);
        }
    }

    public void setPageSize(int pageSize) {
        this.pageSize = pageSize;
    }

    public void setDelay(int delayMs) {
        this.delayMs = delayMs;
    }
}
