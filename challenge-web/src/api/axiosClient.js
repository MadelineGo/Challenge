import axios from "axios";

const DEFAULT_API_BASE_URL = "http://localhost:5166/api";

const sanitizeBaseUrl = (url) => {
    if (typeof url !== "string" || url.trim() === "") {
        return null;
    }
    return url.trim().replace(/\/+$/, "");
};

const resolveBaseUrl = () => {
    const runtimeBaseUrl = typeof window !== "undefined"
        ? window.__APP_CONFIG__?.API_BASE_URL
        : null;

    const envBaseUrl = import.meta?.env?.VITE_API_BASE_URL;

    return sanitizeBaseUrl(runtimeBaseUrl)
        || sanitizeBaseUrl(envBaseUrl)
        || sanitizeBaseUrl(DEFAULT_API_BASE_URL);
};

const axiosClient = axios.create({
    baseURL: resolveBaseUrl(),
    headers: {
        "Content-Type": "application/json",
    },
    timeout: 10000,
});

export default axiosClient;
