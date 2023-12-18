﻿import {Component, Vue} from "vue-facing-decorator";
import instance from "@/utils/axios";
import TreeTable from 'primevue/treetable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import { FilterMatchMode, FilterOperator } from 'primevue/api';
import {ref} from "vue";

@Component({
    components: {
        'Tree-table': TreeTable,
        'Column': Column,
        'InputText':InputText,
    }
})

export default class Addressing extends Vue {
    projectEquip:any[] = []
    filters: any = ref({});
    expandedKeys: any = [];
    async mounted() {
        let user = this.$store.state.auth.authentication.profile;
        if (user && user.role == 'user' || user.role == 'admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.get('/Addressing/Index', {headers});
            let data = JSON.parse(response.data);
            console.log(data);
            this.projectEquip = data.ToTreeTable;

            this.projectEquip.forEach(function (item) {
                // @ts-ignore
                this.expandedKeys[item.key] = true;
                console.log(item);
                // @ts-ignore
                // @ts-ignore
                item.Children.forEach(function (elem) {
                    // @ts-ignore
                    this.expandedKeys[elem.key] = true;
                    // @ts-ignore
                    elem.Children.forEach(function (child) {
                        // @ts-ignore
                        this.expandedKeys[child.key] = true;
                    }, this);
                }, this);
            }, this);
        }
    }
}