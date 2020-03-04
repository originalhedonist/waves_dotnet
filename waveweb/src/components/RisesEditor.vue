<template>
    <div>
        
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Rises } from '../dtos';

    import '@/dtos';
    @Component
    export default class RisesEditor extends Vue {
        @Prop() public rises: Rises;
    }

</script>

<style scoped>
    
</style>
