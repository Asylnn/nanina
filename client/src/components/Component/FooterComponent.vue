
<script lang="ts">

import type { PropType } from 'vue';
import Page from '../../classes/page';
import config from '../../../../config.json'
import User from '@/classes/user/user';
import ItemUse from './ItemUse.vue';

export default {
    name : "FooterComponent",
    data() {
        return {
            config: config,
        }
    },
    emits: ["theme-change","page-change"],

}

</script>


<template>
    <footer>
        <div class="category">
            <div class="categoryName">Contact Us</div><br>
            <a class="clickable"  target="_blank" href="https://github.com/Asylnn/nanina">Github</a><br>
            <a class="clickable"  target="_blank" href="https://discord.gg/xPBGjx2UXG" >Discord</a><br>
            <a class="clickable"  target="_blank" href="mailto:nanina.buisness@proton.me">EMail</a><br>
        </div>
        <div class="category">
            <div class="categoryName">Legal Stuff</div><br>
            <span class="clickable" @click="$emit('page-change', 18)">Privacy</span><br>
            <span class="clickable">Terms</span><br>
        </div>
        <div class="category">
            <div class="categoryName">About Us</div><br>
            <a class="clickable" >Who we are</a><br>
            <a class="clickable" >Wiki</a>
        </div>
    </footer>
    
</template>

<style lang="css" scoped>

.category {
    margin: 0px 5vw;
}

.categoryName{
    padding-bottom: 0px;
}

footer{
    position: absolute;
    left: 0;
    bottom: 0;
    width: 100%;
    padding: 20px 0px;
    display:flex;
    margin-top: 20px;
    background-color: rgb(15,15,15);
    place-content: center;
    transform: translateY(140px);
}

</style>