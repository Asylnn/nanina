<script lang="ts">
import User from '@/classes/user/user';
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import type WebSocketReponse from '@/classes/web_socket_response'
import type Loot from '@/classes/loot/loot';
import LootComponent from '@/components/Component/LootComponent.vue';

export default {
    name : "TreePage",
    data(){
        return {
            user_level_rewards: [] as Loot[][],
        }
    },
    props:{
        user:{
            type:User,
            required:true,
            
        }
    },
    components:{
        LootComponent
    },
    mounted(){
        //@ts-ignore
        this.ws.addEventListener(WebsocketEvent.message, (i: Websocket, ev: MessageEvent) => {
			console.log(`received message: ${ev.data}`);
			var res : WebSocketReponse = JSON.parse(ev.data)
			switch (res.type) {
                case "user level rewards":
                    this.user_level_rewards = JSON.parse(res.data)
                    console.log("USER REWARDS")
                    console.log(res.data)
                    console.log(this.user_level_rewards)
                    break;
            }
        })
        
        this.SendToServer("get possible user level up loot", "", this.user.Id)
    },
    methods:
    {
        getCssForPath(i : number)
        {
            let style = `margin-top:${300*i}px;`
            
            if(i <= this.user.lvl - 2)
                style += `height:${200}px;`
            else if(i == this.user.lvl -1)
                style += `height:${200*(this.user.xp/this.user.XpToLvlUp)}px;`
            return style
        },

        getCssForNode(i : number)
        {
            let style = `margin-top:${197 + 300*(i)}px;`
            
           if(i <= this.user.lvl-2)
           {
                style += `background-color:blueviolet;`
                if((this.user.lvlRewards&2**(i+2)) == 0)
                {
                    style += "cursor:pointer;"
                }
           }
            return style
        },
        getCssForRewards(i : number)
        {
            let style = `margin-top:${174 + 300*(i)}px;`
           /*if(i <= this.lvl-2)
                style += `background-color:blueviolet; cursor:pointer;`*/
            return style
        },
        checkRewardAvailability(i: number)
        {
            i += 2;
            return (this.user.lvlRewards&2**i) == 0 && this.user.lvl >= i
        },
        /*getCssForButton(i: number)
        {
            let style = `margin-top:${230 + 300*(i)}px;`
            if(this.checkRewardAvailability(i))
                style += `color:blueviolet;`
            else
                style += `color:gray;`
            return style
        }*/
        requestRewards(lvl: number)
        {
            if(this.checkRewardAvailability(lvl))
            {
                this.SendToServer("get level reward", `${lvl+2}`, this.user.Id)
            }
        }
    }
}


</script>
<template>
    <div id="treeContainer">
        <div v-for="i in [...Array(60).keys()]">
            <div :style="getCssForPath(i)" class="path element"></div>
            <div :style="getCssForRewards(i)" class="rewards">
                <LootComponent :loots="user_level_rewards[i]"></LootComponent>
            </div>
            
            <div @click="requestRewards(i)" :style="getCssForNode(i)" class="node element">
                <div class="hole"></div>
                <div v-if="checkRewardAvailability(i)" class="point"></div>
            </div>
            <!--<div class="claim" :style="getCssForButton(i)">Claim</div>-->
        </div>
        <div class="tree element" id="tree">
        
        </div>
        <div class="tree element" id="setpage">
            
        </div>
        
        
    </div>
</template>

<style lang="css" scoped>

 .rewards
 {
    position:absolute;
    margin-left: 32vw;
 }

.path
{
    position: absolute;
    width:20px;
    background-color: blueviolet;
    z-index: 6;
}

.element
{
    left:50vw;
    transform: translateX(-50%);
}

.node
{
    position:absolute;
    width:106px;
    height:106px;
    border-radius: 500px;
    background-color: gray;
    z-index: 7;
}

.hole
{
    position:relative;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width:65px;
    height:65px;
    border-radius: 64px;
    background-color: black;
    z-index: 8;
}

.point
{
    position:relative;
    top: -41%; /*IDK why this*/
    left: 51%;
    transform: translate(-50%, 50%);
    width:32px;
    height:32px;
    border-radius: 32px;
    background-color: red;
    z-index: 9;
}

.claim
{
    position: absolute;
    left:40vw;
    transform: translateX(-50%);
    font-size: 30px;
}


.tree
{
    
    width: 18px;
    height: 31000px;
}

#tree
{
    position:absolute;
    background-color: gray;
    z-index: 5;
    
}

#setpage
{
    position:sticky;
    z-index: 0;
    
}

</style>