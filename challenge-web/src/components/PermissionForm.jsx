import { useEffect, useMemo, useState } from "react";
import PropTypes from "prop-types";
import {
    Box,
    Button,
    TextField,
    Typography,
    MenuItem,
    Paper,
    Alert,
} from "@mui/material";
import { PermissionTypesApi } from "../api/permissionTypesApi";

export default function PermissionForm({
    mode = "create", // "create" | "edit"
    initialData = null,
    onSubmit,
    onCancel,
}) {
    const initialFormState = useMemo(
        () => ({
            id: "",
            employeeName: "",
            employeeSurname: "",
            permissionTypeId: "",
        }),
        []
    );

    const [form, setForm] = useState(initialFormState);

    const [types, setTypes] = useState([]);
    const [formError, setFormError] = useState("");
    const [submitting, setSubmitting] = useState(false);
    const [typesLoading, setTypesLoading] = useState(true);
    const [typesError, setTypesError] = useState("");

    useEffect(() => {
        let isMounted = true;
        setTypesLoading(true);
        setTypesError("");

        PermissionTypesApi.getAll()
            .then((res) => {
                if (!isMounted) return;
                setTypes(res.data);
            })
            .catch((err) => {
                console.error("Error loading permission types:", err);
                if (!isMounted) return;
                setTypes([]);
                setTypesError("Unable to load permission types. Try refreshing the page.");
            })
            .finally(() => {
                if (isMounted) setTypesLoading(false);
            });

        return () => {
            isMounted = false;
        };
    }, []);

    useEffect(() => {
        if (initialData) {
            setForm({
                id: initialData.id || "",
                employeeName: initialData.employeeName || "",
                employeeSurname: initialData.employeeSurname || "",
                permissionTypeId: initialData.permissionTypeId?.toString() || "",
            });
        } else {
            setForm(initialFormState);
        }
    }, [initialData, initialFormState]);

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFormError("");

        const trimmedName = form.employeeName.trim();
        const trimmedSurname = form.employeeSurname.trim();
        const permissionTypeId = Number(form.permissionTypeId);

        if (!trimmedName || !trimmedSurname || Number.isNaN(permissionTypeId)) {
            setFormError("Please complete all fields before submitting.");
            return;
        }

        setSubmitting(true);

        try {
            const payload = {
                ...form,
                employeeName: trimmedName,
                employeeSurname: trimmedSurname,
                permissionTypeId,
            };

            await onSubmit?.(payload);

            if (mode === "create") {
                setForm(initialFormState);
            }
        } catch (error) {
            console.error("Error submitting permission form:", error);
            setFormError("An unexpected error occurred. Please try again.");
        } finally {
            setSubmitting(false);
        }

    };

    return (
        <Paper sx={{ p: 4, borderRadius: 2 }}>
            <Typography variant="h5" gutterBottom color="primary">
                {mode === "create" ? "Request New Permission" : "Modify Permission"}
            </Typography>

            <Box
                component="form"
                onSubmit={handleSubmit}
                sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 2 }}
            >
                {formError && (
                    <Alert severity="error" onClose={() => setFormError("")}>
                        {formError}
                    </Alert>
                )}

                {typesError && (
                    <Alert severity="warning" onClose={() => setTypesError("")}>
                        {typesError}
                    </Alert>
                )}

                <TextField
                    label="Employee Name"
                    name="employeeName"
                    value={form.employeeName}
                    onChange={handleChange}
                    fullWidth
                    required
                />

                <TextField
                    label="Employee Surname"
                    name="employeeSurname"
                    value={form.employeeSurname}
                    onChange={handleChange}
                    fullWidth
                    required
                />

                <TextField
                    select
                    label="Permission Type"
                    name="permissionTypeId"
                    value={form.permissionTypeId}
                    onChange={handleChange}
                    fullWidth
                    required
                    disabled={typesLoading}
                    helperText={typesLoading ? "Loading permission types..." : undefined}
                >
                    {types.map((type) => (
                        <MenuItem key={type.id} value={type.id}>
                            {type.description}
                        </MenuItem>
                    ))}
                </TextField>

                <Box display="flex" justifyContent="flex-end" gap={2} mt={2}>
                    {onCancel && (
                        <Button variant="outlined" color="primary" onClick={onCancel}>
                            Cancel
                        </Button>
                    )}

                    <Button type="submit" variant="contained" color="primary" disabled={submitting}>
                        {submitting
                            ? "Saving..."
                            : mode === "create"
                                ? "Submit Request"
                                : "Save Changes"}
                    </Button>
                </Box>
            </Box>
        </Paper>
    );
}

PermissionForm.propTypes = {
    mode: PropTypes.oneOf(["create", "edit"]),
    initialData: PropTypes.shape({
        id: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
        employeeName: PropTypes.string,
        employeeSurname: PropTypes.string,
        permissionTypeId: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
    }),
    onSubmit: PropTypes.func,
    onCancel: PropTypes.func,
};
