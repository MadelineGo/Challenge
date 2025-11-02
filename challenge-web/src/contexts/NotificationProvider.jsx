import { useCallback, useMemo, useState } from "react";
import PropTypes from "prop-types";
import { Alert, Snackbar } from "@mui/material";
import { NotificationContext } from "./NotificationContext.js";

export default function NotificationProvider({ children }) {
    const [notification, setNotification] = useState({
        open: false,
        message: "",
        severity: "info",
    });

    const handleClose = (_, reason) => {
        if (reason === "clickaway") return;
        setNotification((prev) => ({ ...prev, open: false }));
    };

    const notify = useCallback((message, severity = "info") => {
        if (!message) return;
        setNotification({ open: true, message, severity });
    }, []);

    const contextValue = useMemo(
        () => ({
            notify,
            notifySuccess: (message) => notify(message, "success"),
            notifyError: (message) => notify(message, "error"),
            notifyWarning: (message) => notify(message, "warning"),
            notifyInfo: (message) => notify(message, "info"),
        }),
        [notify]
    );

    return (
        <NotificationContext.Provider value={contextValue}>
            {children}
            <Snackbar
                open={notification.open}
                autoHideDuration={4000}
                onClose={handleClose}
                anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
            >
                <Alert
                    onClose={handleClose}
                    severity={notification.severity}
                    elevation={6}
                    variant="filled"
                    sx={{ width: "100%" }}
                >
                    {notification.message}
                </Alert>
            </Snackbar>
        </NotificationContext.Provider>
    );
}

NotificationProvider.propTypes = {
    children: PropTypes.node.isRequired,
};
