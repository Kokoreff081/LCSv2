import {Component, Vue} from 'vue-facing-decorator';
import {PfDropdown} from '@profabric/vue-components';

@Component({
    name: 'languages-dropdown',
    components: {
        'pf-dropdown': PfDropdown
    }
})
export default class Languages extends Vue {
    public selectedLanguage: string = null;
    public languages: any = [
        {
            key: 'en',
            flag: 'flag-icon-us',
            label: 'languages.english'
        },
        {
            key: 'ru',
            flag: 'flag-icon-ru',
            label: 'languages.russian'
        },
    ];

    public mounted() {
        this.selectedLanguage = this.$i18n.locale;
    }

    get flagIcon() {
        if (this.selectedLanguage === 'ru') {
            return 'flag-icon-ru';
        }
        return 'flag-icon-us';
    }

    public changeLanguage(langCode: string) {
        if (this.$i18n.locale !== langCode) {
            this.$i18n.locale = langCode;
            this.selectedLanguage = langCode;
        }
    }
}
