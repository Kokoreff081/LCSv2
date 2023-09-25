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
                    <div class="row item bold">
                        <div class="col">{{$t("message.schedulerScenario")}}</div>
                        <div class="col"></div>
                        <div class="col">{{$t("message.schedulerRepeat")}}</div>
                        <div class="col" v-if="repeating == 'dateTime'">{{$t("message.schedulerStart")}}</div>
                        <div class="col">{{$t("message.schedulerDuration")}}</div>
                    </div>
                    <div class="row item bold">
                        <div class="col div-table">{{inputTask.scenarioName}}</div>
                        <div class="col div-table">
                            <input type="radio" id="one" value="dateTime" v-model="repeating" />
                            <label for="one">DateTime (Range maybe)</label>
                            <br />
                            <input type="radio" id="two" value="add" v-model="repeating" />
                            <label for="two">Select week days for repeat this scenario</label>
                            <br />
                            <input type="radio" id="three" value="astronomic" v-model="repeating" />
                            <label for="three">Astronomic sunrize/sunset settings</label>
                            
                        </div>
                        <div class="col div-table">
                            <datepicker v-if="repeating == 'dateTime'" v-model="dateTimes"
                                        :range="true" :dark="false"></datepicker>
                            <week-days v-else-if="repeating == 'add'" :taskWeekDays="inputTask.selectedWeekDays" :mode="repeating"  @valueChanged="onDateValueChanged"></week-days>
                            <div v-else>
                                <input type="radio" id="one" value="1" v-model="timeType" />
                                <label for="one" >Sunrize</label>
                                <br />
                                <input type="radio" id="two" value="2" v-model="timeType" />
                                <label for="two">Sunset</label>
                                <br />
                                <input type="number" v-model="minutes"/>
                            </div>
                        </div>
                        <div class="col div-table"  v-if="repeating == 'dateTime'"><datepicker :timePicker="true"></datepicker></div>
                        
                        <div class="col div-table">{{inputTask.duration}}</div>
                    </div>
                </section>

                <footer class="modal-footer">
                    <div class="scheduler_scenario_buttons">
                        <button type="button"
                                class="btn-green"
                                @click="submit"
                                aria-label="Close modal">
                            Submit
                        </button>
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
    import Datepicker from '@vuepic/vue-datepicker';
    import WeekDaysComponentVue from './WeekDaysComponent.vue';
    import Clock from '../Clock.vue';
    import Dropdown from 'primevue/dropdown';
    export default {
        name: 'ModalDateDayChanged',
        components: {
            'datepicker': Datepicker,
            'week-days': WeekDaysComponentVue,
            'clock': Clock,
            'myselect': Dropdown,
        },
        emits: ['close', 'submitAndClose'],
        props: ['inputTask'],
        data: function () {
            return {
                repeating: 'dateTime',
                dateTimes: null,
                weekDay:0,
                task: this.inputTask,
                minutes: this.inputTask.minutes,
                timeType: this.inputTask.timeType,
            }
        },
        mounted() {
            //console.log(this.inputTask);
            /*setTimeout(() => {
                console.log(this.inputTask);
                this.task = this.inputTask; }, 1000);*/
        },
        methods: {
            close() {
                this.$emit('close');
            },
            submit: function () {
                let data = {};
                if (this.repeating == 'dateTime') {
                    data = { timeType: 0, value: this.dateTimes, taskChanged: true };
                    this.$emit('submitAndClose', data);
                }
                else if (this.repeating == 'add') {
                    data = { timeType: 0, value: this.weekDay, taskChanged: true };
                    this.$emit('submitAndClose', data);
                }
                else {
                    data = { tiemType: this.timeType, value: this.minutes, taskChanged: true };
                    this.$emit('submitAndClose', data);
                }

            },
            toggleLoop: function (task) {
                task.isLooped = !task.isLooped;
                return this.loopedSelector(task);
            },
            loopedSelector: function (task) {
                if (task.isLooped)
                    return 'icon icon-isLooped';
                return 'icon icon-isNoLooped';
            },
            onDateValueChanged: function (data) {
                this.weekDay = data;
            }
        },
    };
</script>