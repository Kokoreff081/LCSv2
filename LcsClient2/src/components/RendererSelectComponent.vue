<template>
    <div class="V-select2">
        <input class="lg"
               :value="!selectedOption ? 'Please select an item' : selectedOption.name" readonly
               @click="click" />
        <ul v-show="open">
            <li class="li_select2" v-for="(option,i) in options"
                @click="selectOption(option,i)">
                {{ option.name }}
            </li>
        </ul>
    </div>
</template>
<script>
    export default {
        name: 'RendererSelectComponent',
        props: {
            options: {
                type: Array
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
            console.log(this.selected);
            this.selectedOption = this.selected;
            document.body.addEventListener("click", this.close);
        },
        beforeDestroy() {
            document.body.removeEventListener("click", this.close);
        },
        methods: {
            selectOption: function (option, i) {
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
