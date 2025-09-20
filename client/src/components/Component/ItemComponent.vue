<script lang="ts">
import Equipment from '@/classes/item/equipment';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import ModifierComponent from './ModifierComponent.vue';
import GridDisplayComponent from './GridDisplayComponent.vue';
import Modifier from '@/classes/modifiers/modifiers';
import config from '../../../../baseValues.json'
import ClientResponseType from '@/classes/client_response_type';
import DisplayComponent from './DisplayComponent.vue';

export default {
    name : "ItemComponent",
    data() {
        return {
            ItemType : ItemType,
            showingUpgradePanel: false,
            publicPath : import.meta.env.BASE_URL,
            config:config, 
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
        DisplayComponent,
    },
    methods:{
        upgrade()
        {
            console.log(this.userID)
            this.SendToServer(ClientResponseType.UpgradeEquipment, this.item.inventoryId.toString(), this.userID!)
        },
        lvlStarsCSS(upgrade : boolean = false)
        {
            return this.item.rarity == (this.item as Equipment).lvl + +upgrade - 2 ? "lvlMax" : ""
        }

    },
    computed:{
        allModifiers()
        {
            let modifiers = this.item.modifiers
            if(this.item.type == ItemType.Equipment) modifiers = modifiers.concat((this.item as Equipment).getAttributeModifiers())

            //Deepcopy of the poor!
            let u = Modifier.compactModifiers(JSON.parse(JSON.stringify(modifiers)))
            return u
        },
        upgradeQuantity()
        {
            return (this.item as Equipment).stat.amount*(config.equipment_main_stat_level_up_multiplicator - 1)
        }
    }
    
}

</script>

<template>
    <DisplayComponent :type="'item'">
        <div id="itemImage" @click="$emit('click')">
            <div><img :src="`${publicPath}/item-image/${item.imgPATH}`"></div>
            <div v-if="item.type == ItemType.Equipment"  id="lvl" >
                <span v-if="!showingUpgradePanel" :class="lvlStarsCSS()">{{ "★".repeat((item as Equipment).lvl) }}</span>
                <span v-else >
                    <span>{{ "★".repeat((item as Equipment).lvl) }}</span> ➔ 
                    <span :class="lvlStarsCSS(true)">{{ "★".repeat((item as Equipment).lvl + 1) }}</span>
                </span>
            </div>
        </div>
        <div id="ItemInfo" @click="$emit('click')">
            {{ $t(`item.${item.id}.name`) }}<br>
            {{ $t(`item.${item.id}.description`) }}<br><br>
            <p v-if="item.type == ItemType.Equipment">
                {{ $t(`item.type.type`) }} : {{ $t(`item.type.${(item as Equipment).piece}`) }}<br>
                {{ $t(`set.set`) }} : {{ $t(`set.${(item as Equipment).setId}.name`) }}<br><br>
                {{ $t(`item.stat`) }}
                <ModifierComponent v-if="showingUpgradePanel" :modifier="(item as Equipment).stat" :upgrade-quantity="upgradeQuantity"></ModifierComponent>
                <ModifierComponent v-else :modifier="(item as Equipment).stat" ></ModifierComponent><br>
                <p v-if="showingUpgradePanel" class="upgrade">{{$t("item.attributes.new")}} </p><br>
                <div v-if="(item as Equipment).attributes.length != 0">
                    <p>{{$t("item.attributes.attribute")}} </p><br>
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
    </DisplayComponent>
</template>

<style lang="css" scoped>

img {
    /*max-width: 25vw;
    max-height: 60vh;*/
    padding: 2.5vw 2.5vw 0.5vw 2.5vw;
    height: 196px;
    width: 196px;
}

#itemImage {
    display: flex;
    flex-direction: column;
    padding-right: 1vw;
}

#ItemInfo
{
    width:50vw
}

@media only screen and (orientation: landscape) {
    #ItemInfo
    {
        width:25vw
    }
}

.lvlMax
{
    color:gold;
}

#lvl {
    font-size:25px;
    text-align: center;
}

.isForEquiping {
    cursor:pointer;
}


</style>