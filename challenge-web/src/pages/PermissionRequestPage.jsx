import { Container } from "@mui/material";
import { PermissionsApi } from "../api/permissionsApi";
import PermissionForm from "../components/PermissionForm";
import { useNavigate } from "react-router-dom";
import useNotification from "../hooks/useNotification.js";

export default function PermissionRequestPage() {
    const navigate = useNavigate();
    const { notifySuccess, notifyError } = useNotification();

    const handleSubmit = async (formData) => {
        try {
            await PermissionsApi.request(formData);
            notifySuccess("Permission created successfully.");
            navigate("/");
        } catch (error) {
            notifyError("Error creating permission. Please try again.");
            throw error;
        }
    };

    return (
        <Container maxWidth="sm" sx={{ mt: 4 }}>
            <PermissionForm mode="create" onSubmit={handleSubmit} onCancel={() => navigate("/")} />
        </Container>
    );
}
