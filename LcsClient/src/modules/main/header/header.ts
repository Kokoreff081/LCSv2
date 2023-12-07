/* eslint-disable @typescript-eslint/no-empty-function */
/* eslint-disable @typescript-eslint/no-unused-vars */
import {Component, Vue} from 'vue-facing-decorator';
import Messages from './messages/messages.vue';
import Notifications from './notifications/notifications.vue';
import Languages from './languages/languages.vue';
import User from './user/user.vue';
import Clock from '../../../components/clock/clock.vue'


@Component({
    components: {
        'messages-dropdown': Messages,
        'notifications-dropdown': Notifications,
        'languages-dropdown': Languages,
        'user-dropdown': User,
        'clock': Clock,
    }
})
export default class Header extends Vue {
    private headerElement: HTMLElement | null = null;
    public playingNow:string = "---//---";
    public nextToPlay:string = "---//---";
    public async mounted(): Promise<void> {
        this.headerElement = document.getElementById(
            'main-header'
        ) as HTMLElement;
    }

    public onToggleMenuSidebar(): void {
        this.$store.dispatch('ui/toggleMenuSidebar');
    }

    public onToggleControlSidebar(): void {
        this.$store.dispatch('ui/toggleControlSidebar');
    }

    get darkModeSelected() {
        return this.$store.getters['ui/darkModeSelected'];
    }

    get navbarVariant() {
        return this.$store.getters['ui/navbarVariant'];
    }
}
