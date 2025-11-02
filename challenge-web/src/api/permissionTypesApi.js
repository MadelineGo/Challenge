import axiosClient from "./axiosClient";

export const PermissionTypesApi = {
    getAll: () => axiosClient.get("/permissionsType"),
};
