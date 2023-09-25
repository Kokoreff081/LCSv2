<template>

    <div class="wrapper">

        <Preloader></Preloader>
        <Navbar @selectLang="selectLang" :playingNow="playingNow" :nextToPlay="nextToPlay"></Navbar>
        <!--        <Sidebar :baseUrl="appBaseUrl" :rdmEnabled="configValues.isRdmEnabled" @showComponent="showComponent"></Sidebar>-->
        <aside class="main-sidebar sidebar-dark-primary elevation-4">
            <!-- Brand Logo -->
            <a href="#" class="brand-link">

                <span class="brand-text font-weight-light">Light Control Service v. 2</span>
            </a>

            <!-- Sidebar -->
            <div class="sidebar">
                <!-- Sidebar user panel (optional) -->
                <div class="user-panel mt-3 pb-3 mb-3 d-flex">
                    <div class="image">

                    </div>
                    <div class="info">
                        <a href="#" class="d-block">{{ user }}</a>
                    </div>
                </div>

                <!-- SidebarSearch Form -->
                <div class="form-inline">
                    <div class="input-group" data-widget="sidebar-search">
                        <input class="form-control form-control-sidebar" type="search" placeholder="Search" aria-label="Search">
                        <div class="input-group-append">
                            <button class="btn btn-sidebar">
                                <i class="fas fa-search fa-fw"></i>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Sidebar Menu -->
                <nav class="mt-2">
                    <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu" data-accordion="false">
                        <li class="nav-item menu-open">
                            <router-link to="/" class="nav-link active">
                                <i class="nav-icon fas fa-tachometer-alt"></i>
                                <p>
                                    {{$t('message.dashboard')}}
                                    <i class="right fas fa-angle-left"></i>
                                </p>
                            </router-link>
                            <!--<router-link to="/">Dashboard</router-link>-->
                        </li>
                        <li class="nav-item" v-if="!deviceEnabled">
                            <a href="#" class="nav-link">
                                <i class="nav-icon fas fa-th"></i>
                                <p>
                                    {{$t('message.devices')}}
                                    <i class="right fas fa-angle-left"></i>
                                    <!--<span class="right badge badge-danger">New</span>-->
                                </p>
                            </a>
                            <ul class="nav nav-treeview">
                                <li class="nav-item">
                                    <router-link to="/deviceslist" class="nav-link" >
                                        <i class="far fa-circle nav-icon"></i>
                                        <p>Devices v1</p>
                                    </router-link>
                                    <!-- <router-link to="/deviceslist">{{$t('message.devices')}}</router-link>-->
                                </li>
                                <li class="nav-item">
                                    <a href="#" class="nav-link">
                                        <i class="far fa-circle nav-icon"></i>
                                        <p>Devices v2</p>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li class="nav-item">
                            <a href="#" class="nav-link">
                                <i class="nav-icon fas fa-copy"></i>
                                <p>
                                    {{$t('message.project')}}
                                    <i class="fas fa-angle-left right"></i>
                                </p>
                            </a>
                            <ul class="nav nav-treeview">
                                <li class="nav-item">
                                    <router-link to="/project" class="nav-link">
                                        <i class="far fa-circle nav-icon"></i>
                                        <p>{{$t('message.project')}}</p>
                                    </router-link>
                                    <!--<router-link to="/project">{{$t('message.project')}}</router-link>-->
                                </li>
                                <li class="nav-item">
                                    <router-link to="/scheduler" class="nav-link" @click="$emit('showComponent', 'scheduler')">
                                        <i class="far fa-circle nav-icon"></i>
                                        <p>{{$t('message.projectProjectScheduler')}}</p>
                                    </router-link>
                                </li>
                                <li class="nav-item">
                                    <router-link to="/renderer" class="nav-link" >
                                        <i class="far fa-circle nav-icon"></i>
                                        <p>{{$t('message.projectProjectScenPlayer')}}</p>
                                    </router-link>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </nav>
                <!-- /.sidebar-menu -->
            </div>
            <!-- /.sidebar -->
        </aside>
        <div class="content-wrapper" style="height:85%">
            <section class="content">
                <div class="container-fluid">
                    <router-view></router-view>
                </div>
            </section>
        </div>
        <Footer></Footer>
        <ControlSidebar></ControlSidebar>
    </div>
</template>
<script>
import * as $ from "jquery";
import { HTTP } from './global/commonHttpRequest';
import Devices from './components/devices/Devices.vue';
import Project from './components/Project/Project.vue';
import Scheduler from './components/Project/Scheduler.vue';
import Renderer from './components/Project/Renderer.vue';
import Main from './components/MainComponent.vue';
import Graphic from './components/GraphicsComponent/GraphicComponent.vue';
import Clock from './components/common/Clock.vue';
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

import Dropdown from 'primevue/dropdown';
import "../node_modules/admin-lte/plugins/select2-bootstrap4-theme/select2-bootstrap4.min.css"
import "../node_modules/admin-lte/plugins/select2/css/select2.min.css"

import Preloader from "./dashboard/Preloader.vue"
import ContentHeader from "./dashboard/ContentHeader.vue"
import Navbar from "./dashboard/Navbar.vue";
import Footer from "./dashboard/Footer.vue";
import ControlSidebar from "./dashboard/ControlSidebar.vue";

export default {

    name: 'app',
    components: {
        'devices': Devices,
        'project': Project,
        /*'log': Log,*/
        'clock': Clock,
        'graph': Graphic,
        //'projectChanger': ProjectChanger,
        'myselect': Dropdown,
        'scheduler': Scheduler,
        'renderer': Renderer,
        Preloader,
        ContentHeader,
        Navbar,
        Footer,
        ControlSidebar,
        Main
    },
    data() {
        return {
            //tabs: ['ProjectTab', 'DevicesTab', 'LogTab', 'GraphicTab'],
            selectedTab: 'showMain',
            lamps: [],
            devices: Object,
            artnetGatewayNode: [],
            artnetUniverse: [],
            rdm: [],
            errors: [],
            polling: null,
            playingNow: "---//---",
            nextToPlay: "---//---",
            configValues: Object,
            isBadgerVisible: false,
            logEntriesCount: 0,
            projects: null,
            currentProject: null,
            user: this.$cookies.get('UserName'),
            selectedLang: { value: 'ru', text: 'русский' },
            appBaseUrl: null,
            showMain: true,
            showProject: false,
            showScheduler: false,
            showScenarios: false,
            showDevices: false,
            project: Object,
            projectName:null,
            allProjections: [],
            groups: [{ id: 0, name: 'All rasters' }],
            scenarioGroups: [],
            connFlag: false,
            connection: null,
            rastersToRender: null,
            scenariosToScheduler: null,
            isProjectInfoVisible: false,
            projectInfo: Object,
            playScenarioId: -1,
            isOpen: true,
            parentComp: 'projectComponent',
            treeView: true,
            selectedScenario: null,
            selectedScenTicks: 0,
            scenarioToPlay: null,
            expandedKeys: {},
            schedulerFiles: null,
            columns: ['name', 'type', 'lampAddress', 'colorsCount', 'ipAddress', 'parentPort'],
        }
    },
    created() {
        setTimeout(() => {
            HTTP.get('/Configuration')
                .then(response => {
                    this.configValues = response.data;
                    console.log(this.configValues);
                    this.appBaseUrl = this.configValues.baseUrl;
                    if (this.configValues.newLogEntries > 0) {
                        $('#notifier').removeClass('no-notifiers');
                        $('#notifier').addClass('notified');
                        $('#notifier').text(this.configValues.newLogEntries);
                    }
                })
                .catch(e => {
                    console.log(e);
                });
        }, 5);
        /*HTTP.get('/Project').then(response => {
            this.project = response.data;
            console.log(this.project);
            this.projectName = this.project.name;
            this.rastersToRender = this.project.rasters;
            this.project.rasters.forEach(elem => this.groups.push({ id: elem.id, name: elem.name }));
            this.project.scenarios.forEach(elem => this.scenarioGroups.push({ id: elem.scenarioId, name: elem.scenarioName }));
            this.selectedScenario = this.project.scenarios[0];
            this.schedulerFiles = this.project.schedulerFiles

            this.selectedScenTicks = this.selectedScenario.totalTicks;
            this.project.toTreeTable.forEach(function (item) {
                this.expandedKeys[item.key] = true;
                item.children.forEach(function (elem) {
                    this.expandedKeys[elem.key] = true;
                }, this);
            }, this);
        });
        HTTP.get('/Project/ProjectInfo').then(response => {
            console.log(response.data);
            //this.project = JSON.parse(response.data);
            this.projectInfo = response.data;
        });*/
        this.connection = new HubConnectionBuilder()
            .withUrl("/api/lchub")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
        this.connection.start().then(() => {
            this.connFlag = true;
        }).catch(err => { console.error(err.toString()) });


    },
    mounted() {

        this.connection.on('TasksChanged', (info) => {
            let that = this;
            console.log(info);
            that.playingNow = info.currentScheduleTask;
            that.nextToPlay = info.nextScheduleTask;

        });
        this.connection.off('TasksChanged', () => {
            let that = this;
            this.playingNow = "---//---";
            that.nextToPlay = "---//---";
        });


    },
    methods: {
        refreshingProject: function () {

            setTimeout(() => { window.location.reload(); }, 1000);
        },
        selectLang: function (selectedLanguage) {
            console.log(selectedLanguage);
            this.$i18n.locale = selectedLanguage;
        },
        cleanNewEntriesCount: function () {
            $('#notifier').removeClass('notified');
            $('#notifier').addClass('no-notifiers');
            $('#notifier').text('0');

        },
    },
    watch: {
        configValues: {
            handler() {

            },
            immediate: true
        },
    }

}
</script>
<style>
@import url("https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,400i,700&display=fallback");
</style>
