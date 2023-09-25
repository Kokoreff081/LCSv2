import { createWebHistory, createWebHashHistory, createRouter } from "vue-router";
import store from '../store';
import Main from '../components/MainComponent.vue';
import Project from '../components/Project/Project.vue';
import Renderer from '../components/Project/Renderer.vue';
import Scheduler from '../components/Project/Scheduler.vue';
import Devices from '../components/devices/Devices.vue';
import Login from '../dashboard/Login.vue';

const routes = [
    {
        path: "/",
        name: "index",
        component: Main,
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/project",
        name: "project",
        component: Project,
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/scheduler",
        name: "scheduler",
        component: Scheduler,
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/renderer",
        name: "renderer",
        component: Renderer,
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/deviceslist",
        name: "devices",
        component: Devices,
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/login",
        name: "login",
        component: Login
    }
];
const router = createRouter({
    history: createWebHashHistory(),
    routes,
});
router.beforeEach(async (to, from, next) => {
    let storedAuthentication = store.getters['auth'];

    if (to.meta.requiresAuth && !storedAuthentication) {
        return next('/login');
    }
    return next();
});


export default router