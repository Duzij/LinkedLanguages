import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Learn } from "./components/Learn";
import { Setup } from "./components/Setup";
import { Home } from "./components/Home";

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
    path: '/setup',
    requireAuth: true,
    element: <Setup />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
