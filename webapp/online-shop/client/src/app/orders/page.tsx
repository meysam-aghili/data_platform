"use client";

import Box from "@/components/Box";
import { fetchData } from "@/shared/helpers/apiHelper";
import { useEffect, useState } from "react";
import { Package } from "lucide-react";

type OrderType = {
  id: number;
  orderNumber: string;
  status: string;
  total: number;
  createdAt: string;
  items: Array<{
    id: number;
    productTitle: string;
    quantity: number;
    price: number;
  }>;
};

const OrdersPage = () => {
  const [orders, setOrders] = useState<OrderType[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const loadOrders = async () => {
      try {
        const token = localStorage.getItem("auth_token");
        if (!token) {
          // Redirect to login if not authenticated
          window.location.href = "/login";
          return;
        }

        // Fetch orders from API
        // const data = await fetchData<OrderType[]>({
        //   url: "orders",
        // });
        // setOrders(data);
        
        // For now, show empty state
        setOrders([]);
      } catch (error) {
        console.error("Failed to load orders:", error);
      } finally {
        setLoading(false);
      }
    };

    loadOrders();
  }, []);

  if (loading) {
    return (
      <div className="min-h-[calc(100vh-200px)] flex items-center justify-center py-12 px-4">
        <div className="text-gray-500">Loading orders...</div>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-200px)] py-12 px-4">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">My Orders</h1>

        {orders.length === 0 ? (
          <Box>
            <div className="flex flex-col items-center justify-center py-12 text-center">
              <Package className="w-16 h-16 text-gray-300 mb-4" />
              <h2 className="text-xl font-semibold text-gray-700 mb-2">No orders yet</h2>
              <p className="text-gray-500 mb-6">You haven't placed any orders yet.</p>
              <a
                href="/products"
                className="bg-gray-800 hover:bg-gray-900 text-white px-6 py-2 rounded-md transition-colors"
              >
                Start Shopping
              </a>
            </div>
          </Box>
        ) : (
          <div className="flex flex-col gap-4">
            {orders.map((order) => (
              <Box key={order.id}>
                <div className="p-4 w-full">
                  <div className="flex items-center justify-between mb-4">
                    <div>
                      <h3 className="font-semibold text-gray-800">
                        Order #{order.orderNumber}
                      </h3>
                      <p className="text-sm text-gray-500">
                        {new Date(order.createdAt).toLocaleDateString()}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-gray-800">${order.total.toFixed(2)}</p>
                      <span className="text-xs px-2 py-1 bg-gray-100 text-gray-600 rounded">
                        {order.status}
                      </span>
                    </div>
                  </div>
                  <div className="border-t border-gray-200 pt-4">
                    <h4 className="text-sm font-medium text-gray-700 mb-2">Items:</h4>
                    <ul className="space-y-1">
                      {order.items.map((item) => (
                        <li key={item.id} className="text-sm text-gray-600">
                          {item.productTitle} × {item.quantity} - ${item.price.toFixed(2)}
                        </li>
                      ))}
                    </ul>
                  </div>
                </div>
              </Box>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default OrdersPage;

