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
    currentFile : File = null;
    projectName: string = null;
    projectVersions:[] = [];
    lastModified:Date = null;
    selectedVersion:any = null;
    selectedFiles:any[] = [];
    
    async mounted(){
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.get('/Project/Index', {headers});
            let data = JSON.parse(response.data);
            console.log(data);
            this.projectName = data.Name;
            this.projectVersions = data.Versions;
            this.lastModified = data.LastModified;
            let projectVersion:any = this.projectVersions.find((item:any)=>item.Name === data.Path);
            console.log(projectVersion);
            this.selectedVersion = projectVersion.Id;
        }
    }
    
    async onVersionSelect(){
        console.log(this.selectedVersion);
        let selectedProjectVersion = this.projectVersions.find((item:any) => item.Id == this.selectedVersion);
        console.log(selectedProjectVersion);
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.post("/Project/ChangeProjectVersion", selectedProjectVersion, {headers});
            let data = JSON.parse(response.data);
            console.log(data);
            this.projectName = data.Name;
            this.projectVersions = data.Versions;
            this.lastModified = data.LastModified;
            let projectVersion:any = this.projectVersions.find((item:any)=>item.Name === data.Path);
            console.log(projectVersion);
            this.selectedVersion = projectVersion.Id;
        }
    }
    onFileChanged($event: Event) {
        const target = $event.target as HTMLInputElement;
        if (target && target.files) {
            this.currentFile = target.files[0];
            console.log(this.currentFile.name);
        }
    }
    async uploadFile(){
       
        console.log(this.currentFile.name);
        let formData = new FormData();

        formData.append("file", this.currentFile);
        console.log(formData);
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.post("/Project/UploadProject", formData, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            });
            let data = JSON.parse(response.data);
            console.log(data);
            this.projectName = data.Name;
            this.projectVersions = data.Versions;
            this.lastModified = data.LastModified;
            let projectVersion:any = this.projectVersions.find((item:any)=>item.Name === data.Path);
            console.log(projectVersion);
            this.selectedVersion = projectVersion.Id;
        }
    }


}