import { appConfig } from "@/configs";
import { UnauthorizedError } from "../Errors";



export const fetchData = async<T>({
  url,
  params,
  token
}: {
  url: string;
  params?: Record<string, string>;
  token?: string;
}) => {

  let fullUrl = `${appConfig.api_base_url}/${url}`;

  // If parameters exist, build query string
  if (params && Object.keys(params).length > 0) {
    const searchParams = new URLSearchParams();

    for (const [key, value] of Object.entries(params)) {
      if (value !== undefined && value !== null && value !== "") {
        searchParams.append(key, value);
      }
    }

    fullUrl += `?${searchParams.toString()}`;
  }

  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(fullUrl, { cache: "no-store", headers });

  if (!response.ok) {
    const error = `Failed to fetch: ${fullUrl} - status: ${response.status} — error ${await response.text() || "Unknown error"}`;
    if (response.status === 401) {
      throw new UnauthorizedError(error);
    }
    throw new Error(error);
  }
  const data: T = await response.json()

  return data;
};

export const postData = async<T>({
  url,
  body,
  params,
  token
}: {
  url: string;
  body?: Record<string, unknown>;
  params?: Record<string, string>;
  token?: string;
}) => {
  let fullUrl = `${appConfig.api_base_url}/${url}`;

  // If parameters exist, build query string
  if (params && Object.keys(params).length > 0) {
    const searchParams = new URLSearchParams();

    for (const [key, value] of Object.entries(params)) {
      if (value !== undefined && value !== null && value !== "") {
        searchParams.append(key, value);
      }
    }

    fullUrl += `?${searchParams.toString()}`;
  }

  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(fullUrl, {
    method: "POST",
    headers,
    body: body ? JSON.stringify(body) : undefined,
    cache: "no-store",
  });

  const text = await response.text();

  if (!response.ok) {
    const error = `Failed to fetch: ${fullUrl} - status: ${response.status} — error ${text || "Unknown error"}`;
    if (response.status === 401) {
      throw new UnauthorizedError(error);
    }
    throw new Error(error);
  }

  if (!text) {
    return {} as T;
  }

  try {
    return JSON.parse(text) as T;
  } catch (err) {
    console.error("JSON parse failed, returning empty object:", err);
    return {} as T;
  }
};


export const deleteData = async<T>({
  url,
  body,
  params,
  token
}: {
  url: string;
  body?: Record<string, unknown>;
  params?: Record<string, string>;
  token?: string;
}) => {
  let fullUrl = `${appConfig.api_base_url}/${url}`;

  // If parameters exist, build query string
  if (params && Object.keys(params).length > 0) {
    const searchParams = new URLSearchParams();

    for (const [key, value] of Object.entries(params)) {
      if (value !== undefined && value !== null && value !== "") {
        searchParams.append(key, value);
      }
    }

    fullUrl += `?${searchParams.toString()}`;
  }

  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(fullUrl, {
    method: "DELETE",
    headers,
    body: body ? JSON.stringify(body) : undefined,
    cache: "no-store",
  });

  const text = await response.text();

  if (!response.ok) {
    const error = `Failed to fetch: ${fullUrl} - status: ${response.status} — error ${text || "Unknown error"}`;
    if (response.status === 401) {
      throw new UnauthorizedError(error);
    }
    throw new Error(error);
  }

  if (!text) {
    return {} as T;
  }

  try {
    return JSON.parse(text) as T;
  } catch (err) {
    console.error("JSON parse failed, returning empty object:", err);
    return {} as T;
  }
};