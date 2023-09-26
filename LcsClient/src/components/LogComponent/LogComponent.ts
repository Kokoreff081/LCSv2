import {Component, Prop, Vue} from 'vue-facing-decorator';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import { FilterMatchMode, FilterOperator } from 'primevue/api';
import Input from "@/components/input/input";
import instance from "@/utils/axios";

@Component({
    components: {
        'app-input': Input,
        'dataTable': DataTable,
        'column': Column,
        'InputText': InputText,
    }
})
export default class LogComponent extends Vue {
    polling:any = null;
    columns = [ 'id','level', 'deviceId', 'description', 'dateTime' ];
    options = {
        paging: true,
        search: {
            return: true,
        },
        order:[[4, 'desc']]
    };
    tableData:any = [];
    filters:any = {
        'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
    };
    
    async mounted(){
        await instance.get('/Logs')
            .then(response => {
                console.log(response.data);
                this.tableData = response.data;
            })
            .catch(e => {
                console.log(e);
            });
    }

    refreshLogs () {
    this.polling = setTimeout(() => {
        this.tableData.splice(0);
        instance.get('/Logs')
            .then(response => {
                for (let i = 0; i < response.data.length; i++) {
                    let item = response.data[i];
                    this.tableData.push(item);
                }

            })
            .catch(e => {
                console.log(e);
            })
    }, 10);
}
}