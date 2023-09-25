<template>
    <transition name="modal-fade">
        <div class="modal-backdrop_SFM">
            <div class="modal_SFM"
                 role="dialog"
                 aria-labelledby="modalTitle"
                 aria-describedby="modalDescription">
                <header class="modal-header_SFM"
                        id="modalTitle">
                    <clock id="worldTime"></clock>
                    <button type="button"
                            class="btn-close"
                            @click="close"
                            aria-label="Close modal">
                        x
                    </button>
                </header>

                <section class="modal-body_SFM"
                         id="modalDescription">
                    <div>
                        <div v-show="hasFiles" v-for="file in fileList">
                            <div class="div-table-id scheduler_scenario_buttons">
                                <span class="icon icon-delTask" @click="delFile(file)"></span>
                                <span class="item" :class="{bold: file.isCurrentFile}">{{file.name}}</span>
                            </div>
                        </div>
                        <div class="scheduler_scenario_buttons">
                            <span><input type="text" v-model="fileName"/></span>
                            <span class="icon" :class="classSelector" @click="addFile"></span>
                        </div>
                    </div>
                </section>
                <footer class="modal-footer_SFM">
                    <div class="scheduler_scenario_buttons">
                        <button type="button"
                                class="btn-green"
                                @click="close"
                                aria-label="Close modal">
                            Close
                        </button>
                    </div>
                </footer>
            </div>
        </div>
    </transition>
</template>
<script>
    import Clock from '../common/Clock.vue';
    import { HTTP } from '../../global/commonHttpRequest';
    export default {
        name: 'ManageSchedulerFilesModal',
        components: {
            'clock': Clock,
        },
        props: ['fileList', 'currentFile'],
        emits: ['close'],
        data: function () {
            return {
                fileName: '',
            }
        },
        computed: {
            hasFiles: function () {
                return this.fileList && this.fileList.length;
            },
           /* isCurrentFile: function (file) {
                return file.isCurrentFile;
            }*/
            classSelector: function () {
                if (this.fileName == '')
                    return 'icon-addTask-inactive';
                return 'icon-addTask';
            }
        },
        methods: {
            close() {
                this.$emit('close');
            },
            delFile: function (file) {
                if (file.isCurrentFile) {
                    alert(this.$t("message.delSchedulerFileAlert"));
                    return;
                }
                let ChangeSchedulerFileRequest = { Index: file.index, Name: file.name };
                let headers = { 'Content-Type': 'application/json' };
                HTTP.post('/Scheduler/DelSchedulerFile', ChangeSchedulerFileRequest, { headers });
            },
            addFile: function () {
                if (this.fileName == '') {
                    alert(this.$t("message.schedulerFileNameEmpty"));
                    return;
                }
                let existFile = this.fileList.find(e => e.name === this.fileName+'.lctt');
                console.log(existFile);
                if (existFile) {
                    alert(this.$t("message.schedulerFileExists"));
                    return;
                }
                let headers = { 'Content-Type': 'application/json' };
                let NewSchedulerFileName = { FileName: this.fileName };
                HTTP.post('/Scheduler/CreateNewSchedulerFile', NewSchedulerFileName, { headers });
            }
        }
    }
</script>