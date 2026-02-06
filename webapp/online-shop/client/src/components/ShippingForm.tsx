"use client"

import { UnauthorizedError } from "@/shared/Errors";
import { deleteData, fetchData, postData } from "@/shared/helpers/apiHelper";
import useAuthStore from "@/stores/AuthStore";
import { ShippingFormInputs, shippingFormSchema, UserType } from "@/types";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowRight, Trash, Trash2 } from "lucide-react";
import { useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { SubmitHandler, useForm, useWatch, Controller } from "react-hook-form";
import Select from 'react-select';

interface StateCities {
  state: string;
  cities: string[];
}

type Option = {
  value: string;
  label: string;
};

const ShippingForm = ({ setShippingForm }: {
  setShippingForm: (data: ShippingFormInputs) => void;
}) => {

  const router = useRouter();
  const [localShippingForm, setLocalShippingForm] = useState<ShippingFormInputs>();
  const { register, handleSubmit, formState: { errors }, reset, control, setValue } = useForm<ShippingFormInputs>({
    resolver: zodResolver(shippingFormSchema),
    defaultValues: localShippingForm
  });
  const [states, setStates] = useState<Array<Option>>([])
  const [cities, setCities] = useState<StateCities[]>([])
  const [user, setUser] = useState<UserType>()
  const selectedState = useWatch({ control, name: "state_title" });
  const [loading, setLoading] = useState<boolean>(true);
  const { token, isAuthenticated, removeToken } = useAuthStore();
  const [selectedUserAddressIndex, setSelectedUserAddressIndex] = useState<number>(-1);
  const arrayToSelectOption = useMemo(
    () => (arr: string[]) => arr.map((s) => ({ value: s, label: s })),
    []
  );

  const loadData = async () => {
    try {
      const [statesData, citiesData, userData] = await Promise.all([
        fetchData<string[]>({ url: "states" }),
        fetchData<StateCities[]>({ url: "cities" }),
        fetchData<UserType>({ url: "user", token }),
      ]);

      setStates(arrayToSelectOption(statesData));
      setCities(citiesData);
      setUser(userData);
    } catch (error) {
      if (error instanceof UnauthorizedError) {
        removeToken();
      } else {
        console.error("Unexpected error:", error);
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // setShippingForm(undefined!);
    loadData();
  }, []);

  useEffect(() => {
    if (localShippingForm) reset(localShippingForm);
  }, [localShippingForm]);

  useEffect(() => {
    if (!user) return;
    if (selectedUserAddressIndex === -1) {
      setLocalShippingForm({
        name: user.title,
        phone: user.phone,
        address: "",
        state_title: "",
        city_title: "",
        postalCode: ""
      });
      return;
    }
    const addr = user.user_addresses?.[selectedUserAddressIndex];
    setLocalShippingForm({
      name: addr?.title || "",
      phone: user.phone,
      address: addr?.address || "",
      state_title: addr?.state_title || "",
      city_title: addr?.city_title || "",
      postalCode: addr?.postal_code || ""
    });
  }, [user, selectedUserAddressIndex]);

  useEffect(() => {
    if (selectedState) {
      if (selectedUserAddressIndex === -1 || selectedState !== user?.user_addresses?.[selectedUserAddressIndex].state_title) {
        setValue("city_title", "");
      }
    }
  }, [selectedState, setValue]);

  const getCitiesByState = (stateName: string) =>
    cities.find((c) => c.state === stateName)?.cities || [];

  const handleShippingForm: SubmitHandler<ShippingFormInputs> = async (data) => {
    setLoading(true);
    if (user && isAuthenticated()) {
      if (
        !user.user_addresses ||
        user.user_addresses.length === 0 ||
        selectedUserAddressIndex === -1 ||
        user.user_addresses[selectedUserAddressIndex].title !== data.name ||
        user.user_addresses[selectedUserAddressIndex].address !== data.address ||
        user.user_addresses[selectedUserAddressIndex].state_title !== data.state_title ||
        user.user_addresses[selectedUserAddressIndex].city_title !== data.city_title ||
        user.user_addresses[selectedUserAddressIndex].postal_code !== data.postalCode
      ) {
        await postData({
          url: "user-address", body: {
            "state_title": data.state_title,
            "city_title": data.city_title,
            "postal_code": data.postalCode,
            "address": data.address,
            "title": data.name
          }, token
        })
      }
    }
    setLoading(false);
    setShippingForm(data);
    router.push("/cart?step=3", { scroll: false });
  };

  const handleDeleteUserAddress = async (index: number) => {
    const id = user?.user_addresses?.[index].id;
    if (id) {
      await deleteData({ url: `user-address/${id}`, token: token })
      await loadData()
      setSelectedUserAddressIndex(-1);
    }
  }

  if (loading) return null;

  return (
    <div>
      {user?.user_addresses && user.user_addresses.length > 0 && (
        <div className="p-4 border rounded-md bg-gray-50 mb-4 border-gray-300">
          <h3 className="text-sm font-semibold mb-2">Select an address</h3>

          <div className="flex flex-col gap-2">
            {user.user_addresses.map((addr, index) => (
              <label
                key={index}
                className={`flex justify-between p-3 border rounded-md cursor-pointer text-sm ${selectedUserAddressIndex === index
                  ? "border-blue-500 bg-blue-50"
                  : "border-gray-300"
                  }`}
              >
                <div>
                  <input
                    type="radio"
                    name="selectedAddress"
                    value={index}
                    className="mr-2"
                    checked={selectedUserAddressIndex === index}
                    onChange={() => {
                      setSelectedUserAddressIndex(index);
                    }}
                  />
                  {addr.state_title}, {addr.city_title}, {addr.address}, {addr.postal_code}
                </div>
                <button className="p-1.5 cursor-pointer bg-gray-200 rounded-md" onClick={() => handleDeleteUserAddress(index)}>
                  <Trash2 size={15} className="text-red-500" />
                </button>
              </label>

            ))}
            <label
              className={`p-3 border rounded-md cursor-pointer text-sm ${selectedUserAddressIndex === -1
                ? "border-blue-500 bg-blue-50"
                : "border-gray-300"
                }`}
            >
              <input
                type="radio"
                name="selectedAddress"
                value={-1}
                className="mr-2"
                checked={selectedUserAddressIndex === -1}
                onChange={() => {
                  setSelectedUserAddressIndex(-1);
                }}
              />
              Add New
            </label>
          </div>
        </div>
      )}

      <form
        className="flex flex-col gap-4"
        onSubmit={handleSubmit(handleShippingForm)}
      >
        <div className="flex flex-col gap-1">
          <label htmlFor="name" className="text-xs text-gray-500 font-medium">
            Name <span className="text-red-400">*</span>
          </label>
          <input
            className="border-b border-gray-200 py-2 outline-none text-sm"
            type="text"
            id="name"
            placeholder="Ali Dae"
            {...register("name")}
          />
          {errors.name && (
            <p className="text-xs text-red-500">{errors.name.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-1">
          <label htmlFor="phone" className="text-xs text-gray-500 font-medium">
            Phone <span className="text-red-400">*</span>
          </label>
          <input
            className="border-b border-gray-200 py-2 outline-none text-sm"
            type="text"
            id="phone"
            placeholder="123456789"
            {...register("phone")}
          />
          {errors.phone && (
            <p className="text-xs text-red-500">{errors.phone.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-1">
          <label htmlFor="state" className="text-xs text-gray-500 font-medium">
            State <span className="text-red-400">*</span>
          </label>
          <Controller
            name="state_title"
            control={control}
            render={({ field }) => (
              <div>
                <Select
                  value={states.find(s => s.value === field.value) || null}
                  onChange={(selected) => field.onChange(selected?.value)}
                  options={states}
                  isClearable
                  isSearchable
                  placeholder="Select a state"
                  aria-label="State selector"
                />
              </div>
            )}
          />
          {errors.state_title && (
            <p className="text-xs text-red-500">{errors.state_title.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-1">
          <label htmlFor="city" className="text-xs text-gray-500 font-medium">
            City <span className="text-red-400">*</span>
          </label>
          <Controller
            name="city_title"
            control={control}
            render={({ field }) => (
              <div>
                <Select
                  value={arrayToSelectOption(getCitiesByState(selectedState)).find(s => s.value === field.value) || null}
                  onChange={(selected) => field.onChange(selected?.value)}
                  options={arrayToSelectOption(getCitiesByState(selectedState))}
                  isClearable
                  isSearchable
                  placeholder="Select a city"
                  aria-label="City selector"
                />
              </div>
            )}
          />
          {errors.city_title && (
            <p className="text-xs text-red-500">{errors.city_title.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-1">
          <label htmlFor="address" className="text-xs text-gray-500 font-medium">
            Address <span className="text-red-400">*</span>
          </label>
          <input
            className="border-b border-gray-200 py-2 outline-none text-sm"
            type="text"
            id="address"
            placeholder="123 Main St, Anytown"
            {...register("address")}
          />
          {errors.address && (
            <p className="text-xs text-red-500">{errors.address.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-1">
          <label htmlFor="postalCode" className="text-xs text-gray-500 font-medium">
            Postal code <span className="text-red-400">*</span>
          </label>
          <input
            className="border-b border-gray-200 py-2 outline-none text-sm"
            type="text"
            id="postalCode"
            placeholder="1234567890"
            {...register("postalCode")}
          />
          {errors.postalCode && (
            <p className="text-xs text-red-500">{errors.postalCode.message}</p>
          )}
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-gray-800 hover:bg-gray-900 transition-all duration-300 text-white p-2 rounded-lg cursor-pointer flex items-center justify-center gap-2"
        >
          Continue
          <ArrowRight className="w-3 h-3" />
        </button>
      </form>
    </div>
  );
};

export default ShippingForm;
