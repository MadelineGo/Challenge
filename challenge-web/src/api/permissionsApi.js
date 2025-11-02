import axiosClient from "./axiosClient";

export const PermissionsApi = {
    getAll: ({ pageNumber = 1, pageSize = 10 } = {}) =>
        axiosClient.get("/permissions", {
            params: { PageNumber: pageNumber, PageSize: pageSize },
        }),
    getById: (id) => axiosClient.get(`/permissions/${id}`),
    request: (data) => axiosClient.post("/permissions", data),
    modify: (id, data) => axiosClient.put(`/permissions/${id}`, data),
};
