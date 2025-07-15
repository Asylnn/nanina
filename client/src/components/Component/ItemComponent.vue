<script lang="ts">
import Equipment from '@/classes/item/equipment';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import ModifierComponent from './ModifierComponent.vue';
import GridDisplayComponent from './GridDisplayComponent.vue';
import Modifier from '@/classes/modifiers/modifiers';

export default {
    name : "ItemComponent",
    data() {
        return {
            ItemType : ItemType,
            showingUpgradePanel: false,
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props: {
        isForEquiping: {
            type : Boolean,
            default:true,
            required : false,
        },
        isForLoot: {
            type : Boolean,
            default:false,
            required : false,
        },
        item: {
            type : [Item, Equipment],
            required : true
        },
        userID:{
            type:String,
            required: false,
        },
    },
    components:{
        ModifierComponent,
        GridDisplayComponent,
    },
    methods:{
        upgrade()
        {
            console.log(this.userID)
            this.SendToServer("upgrade equipment", this.item.inventoryId.toString(), this.userID!)
        }
    },
    computed:{
        allModifiers()
        {
            //Deepcopy of the poor!
            let u = Modifier.compactModifiers(JSON.parse(JSON.stringify([...this.item.modifiers, ...(this.item as Equipment)?.getAttributeModifiers()])))
            return u
        }
    }
    
}

</script>

<template>
    <div :class="isForEquiping ? 'isForEquiping' : ''" id="focusedObject" >
        <div id="itemImage">
            <div><img :src="`${publicPath}/item-image/${item.imgPATH}`"></div>
        </div>
        <div id="ItemInfo">
            {{ $t(`item.${item.id}.name`) }}<br>
            {{ $t(`item.${item.id}.description`) }}<br><br>
            <p v-if="item.type == ItemType.Equipment">
                {{ $t(`item.type.type`) }} : {{ $t(`item.type.${(item as Equipment).piece}`) }}<br>
                {{ $t(`set.set`) }} : {{ $t(`set.${(item as Equipment).setId}.name`) }}<br><br>
                {{ $t(`item.stat`) }}
                <ModifierComponent v-if="showingUpgradePanel" :modifier="(item as Equipment).stat" :upgrade-quantity="10"></ModifierComponent>
                <ModifierComponent v-else :modifier="(item as Equipment).stat" ></ModifierComponent><br>
                <div v-if="(item as Equipment).attributes.length != 0">
                    <p>{{$t("attributes.attribute")}} </p><br>
                    <div v-for="attribute in (item as Equipment).attributes">
                        <span>{{ $t(`item.attributes.${attribute.id}`) }}</span>
                    </div><br>
                </div>
                
            </p>
            <div v-if="allModifiers.length != 0">
                <p>{{$t("modifiers.modifier")}} </p><br>
                <div  v-for="modifier in allModifiers">
                    <ModifierComponent v-if="modifier != undefined" :modifier="modifier"></ModifierComponent>
                </div>
            </div>
            
            
            <div v-if="showingUpgradePanel"> <!--Upgrade Items-->
                <br>
                <GridDisplayComponent :elements="[item]" :columns="2" :no-margin="true"></GridDisplayComponent>
                <button class="smallbutton nnnbutton" @click="upgrade()">upgrade</button>
                
                
            </div><br>
            
            <span v-if="item.type == ItemType.Equipment && !isForLoot" class="clickable" @click="showingUpgradePanel = ! showingUpgradePanel">upgrade {{showingUpgradePanel ? "⤴" : "⤵"}}</span>
        </div>
    </div>
</template>

<style lang="css" scoped>

#focusedObject {
    position: fixed;
    /*min-width: 400px;
    min-height: 100px;
    width: 30vw;
    height: 15vh;*/
    border-radius: 15px;
    display: flex;
    flex-direction: row;
    padding: 2vh 2vh;
    z-index: 727;
    top:50%;                                /*Make the display be at the center of the screen*/
    left:50%;
    transform: translate(-50%, -50%);
    background-color: rgb(6, 16, 26);
}

#focusedObject img {
    /*max-width: 25vw;
    max-height: 60vh;*/
    padding: 2.5vw 2.5vw;
    height: 128px;
    width: 128px;
}

#itemImage {
    display: flex;
    flex-direction: row;
}

.isForEquiping {
    cursor:pointer;
}

</style>