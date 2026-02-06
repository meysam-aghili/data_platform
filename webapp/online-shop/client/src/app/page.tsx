import ProductList, { ProductListProps } from "@/components/ProductList";

const HomePage = async ({ searchParams }: {searchParams: Promise<ProductListProps>;}) => {

  const params = await searchParams;

  return (
    <ProductList {...params} />
  );
};

export default HomePage;
