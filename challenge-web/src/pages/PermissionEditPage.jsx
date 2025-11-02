import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { PermissionsApi } from "../api/permissionsApi";
import PermissionForm from "../components/PermissionForm";
import { Container, CircularProgress, Alert, Stack, Button } from "@mui/material";
import useNotification from "../hooks/useNotification.js";

export default function PermissionEditPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { notifyError, notifySuccess } = useNotification();

    const [permission, setPermission] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!id) return;

        let isMounted = true;

        const fetchPermission = async () => {
            setIsLoading(true);
            setError("");

            try {
                const response = await PermissionsApi.getById(id);
                if (!isMounted) return;
                setPermission(response.data);
            } catch (err) {
                console.error("Error fetching permission:", err);
                if (!isMounted) return;
                setPermission(null);
                setError("We couldn't load the requested permission. It may have been removed.");
                notifyError("Error loading the requested permission.");
            } finally {
                if (isMounted) setIsLoading(false);
            }
        };

        fetchPermission();

        return () => {
            isMounted = false;
        };
    }, [id, notifyError]);

    const handleSubmit = async (formData) => {
        try {
            await PermissionsApi.modify(id, formData);
            notifySuccess("Permission updated successfully.");
            navigate("/");
        } catch (error) {
            notifyError("Error updating permission. Please try again.");
            throw error;
        }
    };

    if (isLoading) {
        return (
            <Container sx={{ mt: 4, textAlign: "center" }}>
                <CircularProgress />
            </Container>
        );
    }

    if (error) {
        return (
            <Container maxWidth="sm" sx={{ mt: 4 }}>
                <Stack spacing={2}>
                    <Alert severity="error">{error}</Alert>
                    <Button variant="contained" color="primary" onClick={() => navigate("/")}>
                        Back to permissions
                    </Button>
                </Stack>
            </Container>
        );
    }

    if (!permission) {
        return null;
    }

    return (
        <Container maxWidth="sm" sx={{ mt: 4 }}>
            <PermissionForm
                mode="edit"
                initialData={permission}
                onSubmit={handleSubmit}
                onCancel={() => navigate("/")}
            />
        </Container>
    );
}
