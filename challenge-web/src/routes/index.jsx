import PermissionsPage from "../pages/PermissionsPage.jsx";
import PermissionRequestPage from "../pages/PermissionRequestPage.jsx";
import PermissionEditPage from "../pages/PermissionEditPage.jsx";

export const appRoutes = [
    {
        path: "/",
        element: <PermissionsPage />,
    },
    {
        path: "/permissions/request",
        element: <PermissionRequestPage />,
    },
    {
        path: "/permissions/edit/:id",
        element: <PermissionEditPage />,
    },
];

export default appRoutes;
