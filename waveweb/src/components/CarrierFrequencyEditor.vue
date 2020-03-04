<template>
    <div>
        
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { CarrierFrequency } from '../dtos';
    import '@/dtos';
    @Component
    export default class CarrierFrequencyEditor extends Vue {
        @Prop() public carrierFrequency: CarrierFrequency;
    }

</script>

<style scoped>
    
</style>
