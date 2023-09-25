<template>
    <section v-show="isActive"
             :id="computedId"
             ref="tab"
             :aria-hidden="! isActive"
             :class="panelClass"
             role="tabpanel">
        <slot />
    </section>
</template>

<script>
    import { inject, watch, ref, onBeforeMount, onBeforeUnmount } from 'vue';

export default {
  name: 'Tab',
  props: {
    panelClass: {
      type: String,
      default: 'tabs-component-panel'
    },
    id: {
      type: String,
      default: null
    },
    name: {
      type: String,
      required: true
    },
    prefix: {
      type: String,
      default: ''
    },
    suffix: {
      type: String,
      default: ''
    },
    isDisabled: {
      type: Boolean,
      default: false
      },
      isRendered: {
          type: Boolean,
          default:true
      }
  },
  setup(props) {
      const isActive = ref(false)
      const isRendered = ref(true)
    const tabsProvider = inject('tabsProvider')
    const addTab = inject('addTab')
    const updateTab = inject('updateTab')
    const deleteTab = inject('deleteTab')
    const header = props.prefix + props.name + props.suffix
    const computedId = props.id ? props.id : props.name.toLowerCase().replace(/ /g, '-')
    const hash = '#' + (!props.isDisabled ? computedId : '')
    watch(
      () => tabsProvider.activeTabHash,
      () => {
        isActive.value = hash === tabsProvider.activeTabHash
      }
    )
    watch(() => Object.assign({}, props), () => {
      updateTab(computedId, {
        name: props.name,
        header: props.prefix + props.name + props.suffix,
        isDisabled: props.isDisabled,
        isRendered:props.isRendered,
        hash: hash,
        index: tabsProvider.tabs.length,
        computedId: computedId
      })
    })
    onBeforeMount(() => {
      addTab({
        name: props.name,
        header: header,
        isDisabled: props.isDisabled,
        isRendered: props.isRendered,
        hash: hash,
        index: tabsProvider.tabs.length,
        computedId: computedId
      })
    })
    onBeforeUnmount(() => {
      deleteTab(computedId)
    })
    return {
      header,
      computedId,
      hash,
      isActive,
      isRendered,
    }
  }
};
</script>