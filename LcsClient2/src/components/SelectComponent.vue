<template>
    <div class="V-select">
        <input class="lg"
               :value="!selectedOption ? 'Please select an item' : selectedOption.name" readonly
               @click="click" />
        <ul v-show="open">
            <li class="li_select" v-for="(option,i) in options"
                @click="updateOption(option,i)"
                >
                {{ option.name }}
            </li>
        </ul>
    </div>
</template>
<script>
    export default {
        name: 'SelectComponent',
        props: {
            options: {
                type: []
            },
            selected: {}
        },
        emits:['itemSelected'],
        data() {
            return {
                selectedOption: {},
                open: false,
                selectedItem: null
            };
        },
        created() {
            setTimeout(() => {
                console.log(this.selected);
                this.selectedOption = this.selected;
                document.body.addEventListener("click", this.close);
            }, 1000);
            
        },
        beforeDestroy() {
            document.body.removeEventListener("click", this.close);
        },
        methods: {
            updateOption: function (option, i) {
                console.log(this.options, i);
                this.selectedItem = i;
                this.selectedOption = option;
                this.open = false;
                console.log(option);
                this.$emit("itemSelected", option);
            },
            close: function(e) {
                //console.log(this.$el);
                if (!this.$el.contains(e.target)) this.open = false;
            },
            click: function () {
                this.open = !this.open;
            }
        }
    }
</script>
