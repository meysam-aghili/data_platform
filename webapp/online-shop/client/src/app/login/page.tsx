"use client";

import Box from "@/components/Box";
import { postData } from "@/shared/helpers/apiHelper";
import useAuthStore from "@/stores/AuthStore";
import { LoginFormInputs, LoginCodeInputs, loginFormSchema, loginCodeSchema } from "@/types";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowLeft, ArrowRight, Phone, Shield } from "lucide-react";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { toast } from "react-toastify";

type LoginStep = "inputs" | "code" | "success";

const LoginPage = () => {
  const router = useRouter();
  const [step, setStep] = useState<LoginStep>("inputs");
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [countdown, setCountdown] = useState<number>(0);
  const [phoneNumber, setPhoneNumber] = useState<string>("");
  const { setToken } = useAuthStore();

  const {
    register: registerLogin,
    handleSubmit: handleSubmitLogin,
    formState: { errors: loginErrors },
  } = useForm<LoginFormInputs>({
    resolver: zodResolver(loginFormSchema),
  });

  const {
    register: registerCode,
    handleSubmit: handleSubmitCode,
    formState: { errors: codeErrors },
  } = useForm<LoginCodeInputs>({
    resolver: zodResolver(loginCodeSchema),
  });

  const handleSendSms = async (phone: string) => {
    try {
      await postData({
        url: "auth/send-verification-code",
        body: { to: phone },
      });
      setCountdown(120);
      const timer = setInterval(() => {
        setCountdown((prev) => {
          if (prev <= 1) {
            clearInterval(timer);
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
      toast.success("SMS code sent to your phone number");
    } catch (error) {
      throw new Error("Failed to send SMS code");
    }
  }

  const handleLoginSubmit: SubmitHandler<LoginFormInputs> = async (data) => {
    setIsLoading(true);
    try {
      await handleSendSms(data.phone);
      setPhoneNumber(data.phone);
      setStep("code");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to login");
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendSms = async () => {
    if (!phoneNumber) return;
    await handleSendSms(phoneNumber);
  }

  const handleCodeSubmit: SubmitHandler<LoginCodeInputs> = async (data) => {
    setIsLoading(true);
    try {
      const result = await postData<{ token: string }>({
        url: "auth/verify-verification-code",
        body: { to: phoneNumber, code: data.code },
      });

      if (result.token) {
        setToken(result.token);
        toast.success("Logged in successfully");
        router.push("/");
      }
      
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Invalid verification code");
    } finally {
      setIsLoading(false);
    }
  };

  const formatCountdown = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  return (
    <div className="min-h-[calc(100vh-200px)] flex items-center justify-center py-12 px-4">
      <div className="w-full max-w-md">
        <Box>
          <div className="p-4 w-full">
            {/* Header */}
            <div className="text-center mb-8">
              <h1 className="text-2xl font-bold text-gray-800 mb-2">
                {step === "inputs" ? "Sign In / Sign Up" : "Verify Code"}
              </h1>
            </div>

            {/* Phone Number Step */}
            {step === "inputs" && (
              <form
                className="flex flex-col gap-6"
                onSubmit={handleSubmitLogin(handleLoginSubmit)}
              >
                <div className="flex flex-col gap-2">
                  <label
                    htmlFor="phone"
                    className="text-xs text-gray-500 font-medium flex items-center gap-2"
                  >
                    <Phone className="w-4 h-4" />
                    Phone Number
                  </label>
                  <input
                    className="border-b border-gray-200 py-3 outline-none text-sm focus:border-gray-800 transition-colors"
                    type="tel"
                    id="phone"
                    placeholder="09123456789"
                    {...registerLogin("phone")}
                  />
                  {loginErrors.phone && (
                    <p className="text-xs text-red-500">{loginErrors.phone.message}</p>
                  )}
                </div>

                <button
                  type="submit"
                  disabled={isLoading}
                  className="shadow-md w-full bg-gray-800 hover:bg-gray-900 disabled:bg-gray-400 disabled:cursor-not-allowed transition-all duration-300 text-white p-3 rounded-md cursor-pointer flex items-center justify-center gap-2"
                >
                  {isLoading ? "Sending..." : "Send Verification Code"}
                  {!isLoading && <ArrowRight className="w-4 h-4" />}
                </button>
              </form>
            )}

            {/* SMS Code Step */}
            {step === "code" && (
              <form
                className="flex flex-col gap-6"
                onSubmit={handleSubmitCode(handleCodeSubmit)}
              >
                <div className="flex flex-col gap-2">
                  <label
                    htmlFor="code"
                    className="text-xs text-gray-500 font-medium flex items-center gap-2"
                  >
                    <Shield className="w-4 h-4" />
                    Verification Code
                  </label>
                  <input
                    className="border-b border-gray-200 py-3 outline-none text-sm focus:border-gray-800 transition-colors text-center text-2xl tracking-widest"
                    type="text"
                    id="code"
                    placeholder="1234"
                    maxLength={8}
                    {...registerCode("code")}
                  />
                  {codeErrors.code && (
                    <p className="text-xs text-red-500">{codeErrors.code.message}</p>
                  )}
                </div>

                <div className="flex flex-col gap-3">
                  <button
                    type="submit"
                    disabled={isLoading}
                    className="shadow-md w-full bg-gray-800 hover:bg-gray-900 disabled:bg-gray-400 disabled:cursor-not-allowed transition-all duration-300 text-white p-3 rounded-md cursor-pointer flex items-center justify-center gap-2"
                  >
                    {isLoading ? "Verifying..." : "Verify & Continue"}
                    {!isLoading && <ArrowRight className="w-4 h-4" />}
                  </button>

                  <button
                    type="button"
                    onClick={() => setStep("inputs")}
                    className="cursor-pointer w-full text-gray-600 hover:text-gray-800 transition-colors p-2 flex items-center justify-center gap-2 text-sm"
                  >
                    <ArrowLeft className="w-4 h-4" />
                    Change Phone Number
                  </button>

                  <div className="text-center">

                    <button
                      type="button"
                      onClick={handleResendSms}
                      disabled={countdown > 0 || isLoading}
                      className="cursor-pointer text-sm text-gray-600 hover:text-gray-800 disabled:text-gray-400 disabled:cursor-not-allowed transition-colors"
                    >
                      {countdown > 0
                        ? `Resend code in ${formatCountdown(countdown)}`
                        : "Resend verification code"}
                    </button>

                  </div>
                </div>
              </form>
            )}
          </div>
        </Box>
      </div>
    </div>
  );
};

export default LoginPage;

