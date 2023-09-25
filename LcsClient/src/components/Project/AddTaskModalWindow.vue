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
                        <div class="col">{{$t("message.schedulerDate")}}</div>
                        <div class="col" v-if="repeating == 'dateTime'">{{$t("message.schedulerStart")}}</div>
                        <div class="col">{{$t("message.schedulerRepeat")}}</div>
                        <div class="col">{{$t("message.schedulerDuration")}}</div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <myselect :options="scenarios" v-model="selectedScenario" @change="onItemSelected" optionLabel="scenarioName" optionValue="scenarioId" :value="useState" :editable="false">
                                <template #value="slotProps">
                                    <div class="p-dropdown-code">
                                        <span v-if="slotProps.value">{{ slotProps.value.scenarioName }}</span>
                                        <span v-else>
                                            {{ slotProps.placeholder }}
                                        </span>
                                    </div>
                                </template>
                            </myselect>
                        </div>
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
    import WeekDaysComponentVue from './WeekDaysComponent.vue';
    import Clock from '../Clock.vue';
    import Dropdown from 'primevue/dropdown';
    import Calendar from 'primevue/calendar';

    export default {
        name: 'AddTaskModalWindow',
        components: {
            'myselect': Dropdown,
            'datepicker': Calendar,
            'week-days': WeekDaysComponentVue,
            'clock': Clock,
        },
        emits: ['close', 'submitAndClose'],
        props: ['scenarios', 'scenarioToPlay', 'groups'],
        data: function () {
            return {
                scheduledScenarios: [],
                repeating: 'dateTime',
                scenarioDuration: null,
                dateTimes: null,
                minutes: Number,
                timeType: 0,
                task: {
                    scenarioId: null,
                    scenarioName: null,
                    isLooped: false,
                    timeType: null,
                    minutes: null,
                    selectedWeekDays: null,
                    specifiedDateTime: null,
                    specifiedDateTimes: [],

                },
                selectedScenario: this.scenarioToPlay,
            }
        },
        mounted() {
            console.log(this.scenarios);
            setTimeout(() => {
                this.scenarioDuration = this.scenarios[0].scenarioTime;
                this.selectedScenario = this.scenarioToPlay;
            }, 2000);
        },
        methods: {
            close() {
                this.$emit('close');
            },
            submit: function () {
                console.log(this.scheduledScenarios[0]);
                this.task.scenarioId = this.scheduledScenarios[0].id;
                this.task.scenarioName = this.scheduledScenarios[0].name;
                console.log(this.dateTimes);
                if (this.repeating == 'dateTime') {
                    if (this.dateTimes.find(elem => elem == null) != 'undefined') {
                        this.task.specifiedDateTimes = this.dateTimes;
                        this.task.specifiedDateTime = this.dateTimes[0];
                    }
                    else {
                        this.task.specifiedDateTime = this.dateTimes[0];
                        this.task.specifiedDateTimes = [];
                    }
                    this.task.selectedWeekDays = 0;
                    this.task.timeType = 0;
                    this.task.minutes = 0;
                }
                else if (this.repeating != 'astronomic') {
                    this.task.specifiedDateTime = this.dateTimes;
                    this.task.timeType = 0;
                    this.task.minutes = 0;
                }
                else {
                    let dateTime = new Date();
                    dateTime.setDate(dateTime.getDate() + 1);
                    this.task.specifiedDateTime = dateTime;
                    if (this.timeType == 'Sunrize')
                        this.task.timeType = 1;
                    else
                        this.task.timeType = 2;
                    this.task.minutes = this.minutes;
                    this.task.selectedWeekDays = 0;
                }
                console.log(this.task);
                this.$emit('submitAndClose', this.task);
            },
            onItemSelected: function (data) {
                console.log(data);
                this.scenarioDuration = data.scenarioTime;
                this.scheduledScenarios.push({ id: data.id, name: data.name });
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
                console.log(data);
                this.task.selectedWeekDays = data.weekDayVal;
                this.dateTimes = data.timer;
            }
        },
    };

</script>