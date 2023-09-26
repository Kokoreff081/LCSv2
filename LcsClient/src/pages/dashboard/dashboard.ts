import {Component, Vue} from 'vue-facing-decorator';
import ProjectChanger from '@/components/projectChanger/projectChanger.vue';
import instance from "@/utils/axios";
import LogComponent from '@/components/LogComponent/LogComponent.vue';
@Component({
    components:{ProjectChanger:ProjectChanger, 'Log':LogComponent}
})
export default class Dashboard extends Vue {
    public currentProject:any;
    

}
