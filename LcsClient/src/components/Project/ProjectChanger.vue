<template>
    <div class="row">
        <div class="col-sm-2">{{ $t("message.currentProject") }}</div>
        <div class="col-sm-2">
            <select class="form-control" v-model="currentProject" @change="onItemSelected">
                <option v-for="project in projects" :value="project.id">
                    {{project.name}}
                </option>
            </select>
        </div>
        <div class="col-sm-2 custom-file">
            <!--<FileUpload mode="basic" name="selectedFiles" @select="selectFile"></FileUpload>-->
            <input type="file" class="custom-file-input" name="selectedFiles" @change="upload" />
            <label class="custom-file-label" for="customFile">Choose file</label>
        </div>
        <div class="col-sm-2">
            <Button @click="upload" class="btn btn-primary btn-block" :disabled="!selectedFiles">
                <i class="fa fa-upload"></i> &nbsp;
                {{$t('message.projectUpload')}}
            </Button>
        </div>
        <div class="col-sm-2">
            <Button class="btn btn-primary btn-block" @click="refreshCurProject">
                <i class="fa fa-edit"></i> &nbsp; 
                {{ $t('message.projectRefreshButton') }}
            </Button>
        </div>
    </div>
</template>
    <script>
        import { HTTP } from '../../global/commonHttpRequest';
        import Button from 'primevue/button';
        import Dropdown from 'primevue/dropdown';
        import FileUpload from 'primevue/fileupload';

        export default {
            name: 'ProjectChanger',
            components: { 'myselect': Dropdown, 'PrimButton': Button, 'FileUpload': FileUpload },
            emits:['refreshProjectEvent'],
            props:['projectsProps', 'current'],
            computed: {
                isFiles: function () { return this.selectedFiles && this.selectedFiles.length > 0; }
            },
            mounted() {
                setTimeout(() => {
                    HTTP.get('/Project/GetAllProjects')
                        .then(response => {
                            console.log(response.data);

                            this.projects = response.data;
                            this.currentProject = this.projects.find(e => e.isCurrent == true).id;


                            console.log(this.projects);
                            console.log(this.currentProject);
                        })
                        .catch(e => { console.log(e); });
                }, 5);
            },
            data: function () {
                return {
                    selectedFiles: null,
                    currentProject: this.current,
                    projects: this.projectsProps,
                    progress: 0,
                    
                }
            },
            methods: {
                selectFile: function (event) {
                    this.selectedFiles = event.files;
                },
                upload: function () {
                    console.log(this.selectedFiles);
                    this.currentFile = this.selectedFiles[0];
                    let formData = new FormData();

                    formData.append("file", this.currentFile);

                    HTTP.post("/Project/UploadProject", formData, {
                        headers: {
                            "Content-Type": "multipart/form-data"
                        }
                    });
                    HTTP.get('/Project/GetAllProjects')
                        .then(response => {
                            this.projects = response.data;
                            this.currentProject = this.projects.find(e => e.isCurrent == true);
                        })
                        .catch(e => { console.log(e); });
                    this.$emit('refreshProjectEvent');
                },
                onItemSelected: function () {
                    console.log(this.currentProject);
                    let projectName = this.projects.find(elem => elem.id == this.currentProject).name;
                    
                    let sendData = { id: this.currentProject, name: projectName };
                    this.currentProject = this.projects.find(elem => elem.id == this.currentProject);
                    let headers = { 'Content-Type': 'application/json' };
                    HTTP.post('/Project/ChangeProject', sendData, { headers });
                    setTimeout(() => {
                        HTTP.get('/Project/GetAllProjects')
                            .then(response => {
                                this.projects = response.data;
                                this.currentProject = this.projects.find(e => e.isCurrent == true);
                            })
                            .catch(e => { console.log(e); });
                    }, 1000);
                },
                refreshCurProject: function () {
                    let headers = { 'Content-Type': 'application/json' };
                    HTTP.post('/Project/RefreshProject', this.currentProject, { headers });
                    this.$emit('refreshProjectEvent');
                }
            },
            watch: {
                
            }

        }
    </script>
