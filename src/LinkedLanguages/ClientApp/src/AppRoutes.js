import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Learn } from "./components/pages/Learn";
import { Setup } from "./components/pages/Setup";
import { Home } from "./components/pages/Home";
import { Test } from './components/pages/Test';

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/learn',
        requireAuth: true,
        element: <Learn />
    },
    {
        path: '/test',
        requireAuth: true,
        element: <Test />
    },
    {
        path: '/setup',
        requireAuth: true,
        element: <Setup />
    },
    ...ApiAuthorzationRoutes
];

export default AppRoutes;
