"use client"

import useAuthStore from "@/stores/AuthStore";
import { useRouter } from "next/navigation";
import { useEffect } from "react";


const LogoutPage = () => {
    const { removeToken } = useAuthStore();
    const router = useRouter();

    useEffect(() => {
        removeToken();
        router.push("/");
    }, [])

    return (
        <>
        </>
    )
}

export default LogoutPage;