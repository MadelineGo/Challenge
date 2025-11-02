import { createTheme } from "@mui/material/styles";

const theme = createTheme({
    palette: {
        primary: {
            main: "#1976d2", // azul estándar de MUI
        },
        secondary: {
            main: "#9c27b0", // morado
        },
        background: {
            default: "#f4f6f8",
        },
    },
    typography: {
        fontFamily: "Roboto, Helvetica, Arial, sans-serif",
    },
    components: {
        MuiButton: {
            styleOverrides: {
                root: {
                    borderRadius: 8,
                    textTransform: "none",
                },
            },
        },
    },
});

export default theme;
