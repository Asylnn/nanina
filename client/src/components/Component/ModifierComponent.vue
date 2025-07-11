<script lang="ts">
import Modifier from '@/classes/modifiers/modifiers';
import StatModifier from '@/classes/modifiers/stat_modifier';


export default {
    name : "ModifierComponent",
    data() {
        return {
            StatModifier: StatModifier,
        }
    },
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
            <span v-if="modifier.operationType==1">
                <span class="stat">+{{ Math.trunc(((modifier.amount)*100)*10)/10}}%</span>
            </span>
            <span v-else>
                <span class="stat" v-if="modifier.stat == StatModifier.CritChance || modifier.stat == StatModifier.CritDamage">+{{ Math.trunc(((modifier.amount)*100)*10)/10}}%</span>
                <span class="stat" v-else>+{{ Math.trunc(modifier.amount)}}</span>
            </span>
            
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