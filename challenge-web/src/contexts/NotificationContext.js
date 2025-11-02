import { createContext } from "react";

export const NotificationContext = createContext({
    notify: () => {},
    notifySuccess: () => {},
    notifyError: () => {},
    notifyWarning: () => {},
    notifyInfo: () => {},
});
