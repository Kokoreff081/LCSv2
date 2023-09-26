import {Component, Prop, Vue} from "vue-facing-decorator";

@Component({
    name: 'device-info',
})
export default class Input extends Vue {
    @Prop() sensors: [];

}