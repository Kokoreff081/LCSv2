<template>
    <div>
        <div class="col-xl-12">
            <h3 class="rowPadd">{{$t("message.project")}}</h3>
            <!--<tabs :cache-lifetime="120" :options="{ useUrlFragment: false, defaultTabHash: 'projectDevices' }" class="item">
                <tab :name="$t('message.projectProjectDevices')">-->
                    <div>
                        <div class="col-xl-12">
                            <ul>
                                <li>
                                    <div class="bold">
                                        <span @click="showProjectInfo()" style="cursor: grab;">
                                            {{project.name}}
                                        </span><br />
                                    </div>
                                </li>
                                <li>
                                    <div v-if="treeView">
                                        <tree-table :value="project.toTreeTable" class="item p-treetable-sm"
                                                    selectionMode="single" :expandedKeys="expandedKeys" :filters="filters" filterMode="lenient" filterLocale="en"
                                                    @nodeSelect="onNodeSelect" :scrollable="true" scrollHeight="75vh">
                                            <template #header>
                                                <div style="display:flex;">
                                                    <div class="text-left" style="margin-right:42%;">
                                                        <span class="icon icon-table_rows-switch" @click="toggleViewToTable"></span>
                                                        <span class="icon icon-account_tree-switch" @click="toggleViewToTree"></span>
                                                    </div>
                                                    <div class="text-right" style="margin-left:42%;">
                                                        <div class="p-input-icon-left">
                                                            <i class="pi pi-search"></i>
                                                            <InputText v-model="filters['global']" placeholder="Global Search" size="50" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </template>
                                            <column field="name" header="" :expander="true" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t('message.deviceName')}}</span>
                                                </template>
                                            </column>
                                            <column field="dmxAddress" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t('message.deviceDmxStartAddress')}}</span>
                                                </template>
                                            </column>
                                            <column field="dmxFootprint" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t('message.deviceFootprint')}}</span>
                                                </template>
                                            </column>
                                        </tree-table>
                                    </div>
                                    <div v-else>
                                        <DataTable :value="project.onlyProjectLamps" :paginator="true" :rows="10" :rowsPerPageOptions="[10,20,50]" v-model:filters="filtersDT"
                                                   :globalFilterFields="columns" :scrollable="true" scrollHeight="50vh">
                                            <template #header>
                                                <div style="display:flex;">
                                                    <div class="text-left" style="margin-right:42%;">
                                                        <span class="icon icon-table_rows-switch" @click="toggleViewToTable"></span>
                                                        <span class="icon icon-account_tree-switch" @click="toggleViewToTree"></span>
                                                    </div>
                                                    <div class="text-right" style="margin-left:42%;">
                                                        <div class="p-input-icon-left">
                                                            <i class="pi pi-search"></i>
                                                            <InputText v-model="filters['global']" placeholder="Global Search" size="50" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </template>
                                            <column field="name" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t("message.deviceName")}}</span>
                                                </template>
                                            </column>
                                            <column field="type" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t("message.deviceType")}}</span>
                                                </template>
                                            </column>
                                            <column field="lampAddress" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t("message.deviceDmxStartAddress")}}</span>
                                                </template>
                                            </column>
                                            <column field="colorsCount" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t("message.deviceFootprint")}}</span>
                                                </template>
                                            </column>
                                            <column field="ipAddress" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t("message.deviceIpAddress")}}</span>
                                                </template>
                                            </column>
                                            <column field="parentPort" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                                                <template #header>
                                                    <span>{{$t("message.devicePortAddress")}}</span>
                                                </template>
                                            </column>
                                        </DataTable>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <!--</tab>
                <tab :name="$t('message.projectProjectScheduler')">
            <div>
                <div class="row">
                    <div class="col-xl-12 projectGridSheduler">

                    </div>
                </div>
            </div>
        </tab>
        <tab :name="$t('message.projectProjectScenPlayer')">
            <div>
                <div class="row">
                    <div class="col-xl-12 projectGrid">

                    </div>
                </div>
            </div>
        </tab>
    </tabs>-->
</div>
    </div>   
    <Dialog v-model:visible="isProjectInfoVisible" :breakpoints="{'960px': '75vw', '640px': '90vw'}" :style="{width: '50vw'}" :modal="true">
        <template #header>
            <span> {{$t("message.projectInfoHeader")}}</span>
        </template>
        <div class="item bold">
            <div>
                <span>{{$t("message.projectInfoTitle")}} - {{projectInfo.title}} </span><br />
                <span v-if="project">{{$t("message.projectInfoDate")}} - {{projectInfo.date}}</span><br />
                <span v-if="project.lastModified">{{$t("message.projectInfoLastModify")}} - ({{projectInfo.lastModified}})</span><br />
            </div><br />
            <div v-if="project">
                <span>{{$t("message.projectInfoAssembledByCompany")}} - {{projectInfo.assembledByCompany}}</span><br />
                <span>{{$t("message.projectInfoAssembledByName")}} - {{projectInfo.assembledByName}}</span><br />
                <span>{{$t("message.projectInfoAssembledByEmail")}} - {{projectInfo.assembledByEmail}}</span><br />
                <span>{{$t("message.projectInfoAssembledByPhone")}} - {{projectInfo.assembledByPhone}}</span><br />
                <span>{{$t("message.projectInfoAssembledByAddress")}} - {{projectInfo.assembledByAddress}}</span><br />
            </div><br />
            <div v-if="project">
                <span>{{$t("message.projectInfoCustomerCompany")}} - {{projectInfo.customerCompany}}</span><br />
                <span>{{$t("message.projectInfoCustomerName")}} - {{projectInfo.customerName}}</span><br />
                <span>{{$t("message.projectInfoCustomerEmail")}} - {{projectInfo.customerEmail}}</span><br />
                <span>{{$t("message.projectInfoCustomerPhone")}} - {{projectInfo.customerPhone}}</span><br />
                <span>{{$t("message.projectInfoCustomerAddress")}} - {{projectInfo.customerAddress}}</span><br />
            </div><br />
            <div>
                {{$t("message.projectInfoDescription")}} 
                <p>
                    {{projectInfo.description}}
                </p>
            </div><br />
            <div>
                {{$t("message.projectInfoObjectAddress")}} 
                <p>
                    {{projectInfo.objectAddress}}
                </p>
            </div>
        </div>
        <template #footer>
            <PrimButton label="Close" icon="pi pi-times" @click="closeModal" class="p-button-text" />
        </template>
    </Dialog>

</template>
<script>
    import $ from "jquery";
    import { HTTP } from '../../global/commonHttpRequest';
   
    import ProjectInfo from './ProjectInfoModal.vue';
    import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
    import TreeTable from 'primevue/treetable';
    import Column from 'primevue/column';
    import InputText from 'primevue/inputtext';
    import DataTable from 'primevue/datatable';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import Dialog from 'primevue/dialog';
    import Button from 'primevue/button';

    export default {
        name: 'Project',
        components: {
            'project-info': ProjectInfo,
            'tree-table': TreeTable,
            'column': Column,
            'InputText': InputText,
            'DataTable': DataTable,
            'Dialog': Dialog,
            'PrimButton': Button,
        },
        data: function () {
            return {
                project: Object,
                allProjections: [],
                groups: [{ id: 0, name: 'All rasters' }],
                scenarioGroups: [],
                connFlag : false,
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
                filters: {},
                filtersDT: {
                    'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                schedulerFiles: null,
                columns: ['name', 'type', 'lampAddress', 'colorsCount', 'ipAddress', 'parentPort'],
            }
        },
        watch: {
            rastersToRender: {
                handler() {
                    this.rastersToRender = this.project.rasters;
                },
		        immediate: true
            },
            scenariosToScheduler: {
                handler() {
                    this.scenariosToScheduler = this.project.scenarios;
                },
                immediate: true
            },
        },
        computed: {
            isFolder: function () {
                return this.project.lcLamps && this.project.lcLamps.length;
            },
        },
        created() {
            HTTP.get('/Project').then(response => {
                this.project = response.data;
                console.log(this.project);
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
            });
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
            this.connection.on('ProjectChanged', (newProject) => {
                let that = this;
                Object.assign(that.project, newProject);
                this.groups.splice(0, this.groups.length);
                this.groups.push({ id: 0, name: 'All rasters' });
                this.scenarioGroups.splice(0, this.scenarioGroups.length);
                this.scenarioGroups.push({ id: 0, name: 'All rasters' });
                this.project.rasters.forEach(elem => this.groups.push({ id: elem.id, name: elem.name }));
                this.project.scenarios.forEach(elem => this.scenarioGroups.push({ id: elem.scenarioId, name: elem.scenarioName }));
                this.$forceUpdate();
            });
            this.connection.on('SchedulerFileChanged', (newInfo) => {
                let that = this;
                this.project.scheduler = newInfo.scheduler;
                this.project.schedulerFiles = newInfo.schedulerFiles;
                this.project.isTasksPlaying = newInfo.isTasksPlaying;
            });
            this.connection.on('SchedulerRefresh', (info) => {
                let that = this;
                this.project.scheduler = info.scheduler;
                this.project.isTasksPlaying = info.isTasksPlaying;
            });
            
        },
        methods: {
            showProjectInfo: function () {
                this.isProjectInfoVisible = !this.isProjectInfoVisible;
            },
            closeModal() {
                this.isProjectInfoVisible = false;
            },
            
            toggle: function () {
                if (this.isFolder) {
                    this.isOpen = !this.isOpen;
                }
            },
            toggleViewToTable: function () {
                this.treeView = false;
                $(document).ready(function () {
                    $('#tableViewDevicesProj').DataTable();
                });
            },
            toggleViewToTree: function () {
                this.treeView = true;
            }
        }
    }
</script>
