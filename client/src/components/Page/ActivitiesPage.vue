<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';
import ItemComponent from '../Component/ItemComponent.vue';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import Item from '@/classes/item/item';

export default {
    name : "ActivitiesPage",
    data(){
        return {
            activity : "help",
        }
    },
    props: {
        user: {
            type: User,
            required: true
        }
    },
    methods:{
        applyTextColor(activity : string){
            return activity == this.activity ? "selected" : ""
        }
    },
    components:{
    }
}


</script>
<template>
    <div>
        <ul id="inventoryHeader">
            <li :class="applyTextColor('help') + ' clickable'" @click="activity = 'help'">{{$t("activities.help")}}</li>
            <li :class="applyTextColor('cafe') + ' clickable'" @click="activity = 'cafe'">{{$t("activities.cafe")}}</li>
            <li :class="applyTextColor('mining') + ' clickable'" @click="activity = 'mining'">{{$t("activities.mining")}}</li>
            <li :class="applyTextColor('research') + ' clickable'" @click="activity = 'research'">{{$t("activities.research")}}</li>
            <li :class="applyTextColor('crafting') + ' clickable'" @click="activity = 'crafting'" >{{$t("activities.crafting")}}</li>
        </ul>
    </div>
</template>

<style lang="css" scoped>

#inventoryHeader {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
    height: 5vh;
    margin: 0 20vw;
    text-align: center;
    cursor: pointer;
}

</style>