import { BrowserRouter, Routes, Route } from "react-router-dom";
import appRoutes from "./routes/index.jsx";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {appRoutes.map((route) => (
          <Route key={route.path} path={route.path} element={route.element} />
        ))}
      </Routes>
    </BrowserRouter>
  );
}
