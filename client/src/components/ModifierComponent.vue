<script lang="ts">
import Modifier from '@/classes/modifiers/modifiers';


export default {
    name : "ModifierComponent",
    props: {
        modifier: {
            type : Modifier,
            required : true
            
        },
    },
}

</script>

<template>
    <div>
        <div class="modifier">
            <span>{{ $t(`modifiers.${modifier.stat}.name`) }}</span>
            <span class="stat">+{{ Math.trunc(((modifier.amount)*100)*10)/10}}%</span>
        </div>
    </div>
</template>

<style lang="css" scoped>

.modifier {
    display: grid;
    grid-template-columns: 5fr 1fr;
}

.stat {
    width:60px;
}

</style>