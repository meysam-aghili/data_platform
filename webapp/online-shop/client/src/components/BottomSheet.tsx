"use client";

import { AnimatePresence, motion } from "framer-motion";

export default function BottomSheet({ open, onClose, children }: {
    open: boolean;
    onClose: () => void;
    children: React.ReactNode;
}) {
    return (
        <AnimatePresence>
            {open && (
                <>
                    {/* Background Overlay */}
                    <motion.div
                        className="fixed inset-0 bg-black/40 z-40"
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        onClick={onClose}
                    />

                    {/* Bottom Sheet */}
                    <motion.div
                        className="fixed bottom-0 left-0 right-0 z-50 bg-white rounded-t-md shadow-xl
                       max-h-[85vh] overflow-y-auto"
                        initial={{ y: "100%" }}
                        animate={{ y: 0 }}
                        exit={{ y: "100%" }}
                        transition={{ type: "tween", duration: 0.35 }}
                    >
                        {/* Header */}
                        <div className="sticky top-0 bg-white p-3 border-b flex items-center border-gray-300">
                            <button onClick={onClose} className="text-gray-500 text-xl">×</button>
                        </div>

                        {children}

                    </motion.div>
                </>
            )}
        </AnimatePresence>
    );
}
