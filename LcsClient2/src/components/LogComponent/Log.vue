<template>
    <div class="row align-items-center">
        <div class="col-12">
            <div class="status_legend">
                <div class="status_legend">
                    <h3>{{$t("message.log")}}</h3>
                </div>
                <div class="status_legend">
                    <Button @click="refreshLogs" class="btn btn-info">
                        {{$t('message.logRefresh')}}
                        <i class="fa-thin fa-arrow-rotate-right"></i>
                    </Button>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <dataTable :value="tableData" :paginator="true" :rows="15" :rowsPerPageOptions="[15,30,60]" v-model:filters="filters"
                       :globalFilterFields="columns" :scrollable="true" scrollHeight="70vh">
                <template #header>
                    <div class="flex justify-content-end">
                        <span class="p-input-icon-left ">
                            <i class="pi pi-search" />
                            <InputText v-model="filters['global'].value" placeholder="Keyword Search" />
                        </span>
                    </div>
                </template>
                <column field="id" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                    <template #header>
                        <span>{{$t('message.logId')}}</span>
                    </template>
                </column>
                <column field="level" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                    <template #header>
                        <span>{{$t('message.logLevel')}}</span>
                    </template>
                </column>
                <column field="deviceId" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                    <template #header>
                        <span>{{$t('message.logDeviceId')}}</span>
                    </template>
                </column>
                <column field="description" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                    <template #header>
                        <span>{{$t('message.logDescription')}}</span>
                    </template>
                </column>
                <column field="dateTime" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                    <template #header>
                        <span>{{$t('message.logDateTime')}}</span>
                    </template>
                </column>
            </dataTable>
            <!-- <table id="logs">
         <thead>
             <tr>
                 <th>{{$t("message.logId")}}</th>
                 <th>{{$t("message.logLevel")}}</th>
                 <th>{{$t("message.logDeviceId")}}</th>
                 <th>{{$t("message.logDescription")}}</th>
                 <th>{{$t("message.logDateTime")}}</th>
             </tr>
         </thead>
         <tbody>
             <tr v-for="data in tableData">
                 <td>{{data.id}}</td>
                 <td>{{data.level}}</td>
                 <td>{{data.deviceId}}</td>
                 <td>{{data.description}}</td>
                 <td>{{data.dateTime}}</td>
             </tr>
         </tbody>
     </table>-->
        </div>
    </div>
    <!--</div>
    </div>-->
</template>
<script>
    import { HTTP } from '../../global/commonHttpRequest';
    import $ from "jquery";
    //import DataTable from "datatables.net";
    import DataTable from 'primevue/datatable';
    import Column from 'primevue/column';
    import InputText from 'primevue/inputtext';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import Button from 'primevue/button';

    export default {
        name: 'Log',
        components: {
            'dataTable': DataTable,
            'column': Column,
            'InputText': InputText,
            'PrimButton': Button,
        },
        emits:['cleanBadger'],
        data: function () {
            return {
                polling: null,
                columns: [ 'id','level', 'deviceId', 'description', 'dateTime' ],
                options: {
                    paging: true,
                    search: {
                        return: true,
                    },
                    order:[[4, 'desc']]
                },
                tableData: [],
                logs: [],
                filters: {
                    'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
            }
        },
        created() {
            console.log('log is now on develop stage');
            
            /*this.tableData.splice(0);
            HTTP.get('/Logs')
                .then(response => {
                    console.log(response.data);
                    for (let i = 0; i < response.data.length; i++) {
                        let item = response.data[i];
                        this.tableData.push(item);
                    }
                })
                .catch(e => {
                    console.log(e);
                });
            */
            
        },
        mounted() {
            HTTP.get('/Logs')
                .then(response => {
                    console.log(response.data);
                    this.tableData = response.data;
                    $(document).ready(function () {
                        $('#logs').DataTable();
                    });  
                })
                .catch(e => {
                    console.log(e);
                });
                    
        },
        methods: {
            refreshLogs: function () {
                this.polling = setTimeout(() => {
                    this.tableData.splice(0);
                    HTTP.get('/Logs')
                        .then(response => {
                            for (let i = 0; i < response.data.length; i++) {
                                let item = response.data[i];
                                this.tableData.push(item);
                            }
                            this.$emit('cleanBadger');
                        })
                        .catch(e => {
                            console.log(e);
                        })
                }, 10);
            }
        }
    }
</script>
<style>

    @import 'datatables.net-bs5';
</style>