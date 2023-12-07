import {Component, Prop, Vue} from 'vue-facing-decorator';
import instance from '../../utils/axios';
import Input from '@/components/input/input.vue';
import Dropdown from "primevue/dropdown";
import {date} from "yup";

@Component({
    components: {
        'app-input': Input,
        'Dropdown':Dropdown
    }
})
export default class ProjectChanger extends Vue {
    currentFile : any = null;
    projectName: string = null;
    projectVersions:[] = [];
    lastModified:Date = null;
    selectedVersion:any = null;
    async mounted(){
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.get('/Project/Index', {headers});
            var data = JSON.parse(response.data);
            this.projectName = data.Name;
            this.projectVersions = data.Versions;
            this.lastModified = data.LastModified;
            this.selectedVersion = data.Path;
        }
    }
}