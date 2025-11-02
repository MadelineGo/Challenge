import { useCallback, useEffect, useState } from "react";
import {
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    Paper, TablePagination, Button
} from "@mui/material";
import { PermissionsApi } from "../api/permissionsApi";
import { useNavigate } from "react-router-dom";

export default function PermissionTable() {
    const [permissions, setPermissions] = useState([]);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const navigate = useNavigate();

    const loadData = useCallback(async (pageNumber, pageSize) => {
        setLoading(true);
        setError(null);
        try {
            const response = await PermissionsApi.getAll({
                pageNumber: pageNumber + 1,
                pageSize,
            });
            const { data } = response;
            const permissionsData = data.permissions ?? data;
            const normalizedPermissions = Array.isArray(permissionsData) ? permissionsData : [];

            setPermissions(normalizedPermissions);

            const count = typeof data.totalCount === "number"
                ? data.totalCount
                : normalizedPermissions.length;
            setTotalCount(count);
        } catch (error) {
            console.error("Error fetching permissions:", error);
            setError("Unable to load permissions. Please try again.");
            setPermissions([]);
            setTotalCount(0);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadData(page, rowsPerPage);
    }, [loadData, page, rowsPerPage]);

    const handleChangePage = (_, newPage) => setPage(newPage);
    const handleChangeRowsPerPage = (e) => {
        setRowsPerPage(parseInt(e.target.value, 10));
        setPage(0);
    };

    return (
        <Paper sx={{ width: "100%", overflow: "hidden", mt: 3 }}>
            <TableContainer sx={{ maxHeight: 440 }}>
                <Table stickyHeader>
                    <TableHead>
                        <TableRow>
                            <TableCell>ID</TableCell>
                            <TableCell>Employee Name</TableCell>
                            <TableCell>Employee Surname</TableCell>
                            <TableCell>Permission Type</TableCell>
                            <TableCell>Created</TableCell>
                            <TableCell align="center">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {loading && (
                            <TableRow>
                                <TableCell colSpan={6} align="center">
                                    Loading permissions...
                                </TableCell>
                            </TableRow>
                        )}

                        {!loading && error && (
                            <TableRow>
                                <TableCell colSpan={6} align="center">
                                    {error}
                                </TableCell>
                            </TableRow>
                        )}

                        {!loading && !error && permissions.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={6} align="center">
                                    No permissions found.
                                </TableCell>
                            </TableRow>
                        )}

                        {!loading && !error && permissions.map((p) => (
                            <TableRow key={p.id} hover>
                                <TableCell>{p.id}</TableCell>
                                <TableCell>{p.employeeName}</TableCell>
                                <TableCell>{p.employeeSurname}</TableCell>
                                <TableCell>{p.permissionTypeDescription}</TableCell>
                                <TableCell>
                                    {p.created ? new Date(p.created).toLocaleDateString() : "—"}
                                </TableCell>
                                <TableCell align="center">
                                    <Button
                                        variant="outlined"
                                        color="primary"
                                        size="small"
                                        onClick={() => navigate(`/permissions/edit/${p.id}`)}
                                    >
                                        Edit
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            <TablePagination
                rowsPerPageOptions={[5, 10, 20]}
                component="div"
                count={totalCount}
                rowsPerPage={rowsPerPage}
                page={page}
                onPageChange={handleChangePage}
                onRowsPerPageChange={handleChangeRowsPerPage}
            />
        </Paper>
    );
}
