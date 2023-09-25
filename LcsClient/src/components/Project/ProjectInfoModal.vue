<template>
    <transition name="modal-fade">
        <div class="modal-backdrop">
            <div class="modal"
                 role="dialog"
                 aria-labelledby="modalTitle"
                 aria-describedby="modalDescription">
                <header class="modal-header"
                        id="modalTitle">
                    <clock id="worldTime"></clock>
                    <button type="button"
                            class="btn-close"
                            @click="close"
                            aria-label="Close modal">
                        x
                    </button>
                </header>

                <section class="modal-body"
                         id="modalDescription">
                    <div class="item bold">
                        <div>
                            <span>{{$t("message.projectInfoTitle")}} - {{project.title}} </span><br />
                            <span v-if="project">{{$t("message.projectInfoDate")}} - {{project.date}}</span><br/>
                            <span v-if="project.lastModified">{{$t("message.projectInfoLastModify")}} - ({{project.lastModified}})</span><br />
                        </div><br />
                         <div v-if="project">
                             <span>{{$t("message.projectInfoAssembledByCompany")}} - {{project.assembledByCompany}}</span><br />
                             <span>{{$t("message.projectInfoAssembledByName")}} - {{project.assembledByName}}</span><br />
                             <span>{{$t("message.projectInfoAssembledByEmail")}} - {{project.assembledByEmail}}</span><br />
                             <span>{{$t("message.projectInfoAssembledByPhone")}} - {{project.assembledByPhone}}</span><br />
                             <span>{{$t("message.projectInfoAssembledByAddress")}} - {{project.assembledByAddress}}</span><br />
                         </div><br />
                         <div v-if="project">
                             <span>{{$t("message.projectInfoCustomerCompany")}} - {{project.customerCompany}}</span><br />
                             <span>{{$t("message.projectInfoCustomerName")}} - {{project.customerName}}</span><br />
                             <span>{{$t("message.projectInfoCustomerEmail")}} - {{project.customerEmail}}</span><br />
                             <span>{{$t("message.projectInfoCustomerPhone")}} - {{project.customerPhone}}</span><br />
                             <span>{{$t("message.projectInfoCustomerAddress")}} - {{project.customerAddress}}</span><br />
                         </div><br />
                         <div>
                             {{$t("message.projectInfoDescription")}} -
                             <p>
                                 {{project.description}}
                             </p>
                         </div><br />
                         <div>
                             {{$t("message.projectInfoObjectAddress")}} -
                             <p>
                                 {{project.objectAddress}}
                             </p>
                         </div>
                    </div>
                    
                </section>

                <footer class="modal-footer">
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
    import { HTTP } from '../../global/commonHttpRequest';
    export default {
        name: "ProjectInfo",
        emits: ['close'],
        data: function () {
            return {
                project: Object,
            }
        },
        mounted() {
            /*setTimeout(() => {*/
                HTTP.get('/Project/ProjectInfo').then(response => {
                    console.log(response.data);
                    //this.project = JSON.parse(response.data);
                    this.project = response.data;
                });
           /* }, 500);*/
        },
        methods: {
            close() {
                this.$emit('close');
            },
        },
    }
</script>