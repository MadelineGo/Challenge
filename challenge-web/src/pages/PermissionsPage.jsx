import { Container, Typography, IconButton, Tooltip, Box } from "@mui/material";
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import PermissionTable from "../components/PermissionTable";
import { useNavigate } from "react-router-dom";

export default function PermissionsPage() {
    const navigate = useNavigate();

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h4" gutterBottom color="primary">
                    Permissions Management
                </Typography>

                <Tooltip title="Add new permission">
                    <IconButton
                        color="primary"
                        size="large"
                        onClick={() => navigate("/permissions/request")}
                    >
                        <AddCircleOutlineIcon fontSize="inherit" />
                    </IconButton>
                </Tooltip>
            </Box>

            <PermissionTable />
        </Container>
    );
}
