<script lang="ts">

export default {
    name : "NotificationMenu",
    props: {
        /*diffLvlup : {
            type : Number,
            required : true
        },*/
    }
}


</script>

<template>
    <div id="notifMenu"></div>
        


        <!-- <select v-model="selected_theme" @change="onChangeTheme()">
            <option value = "dark_theme">Dark Theme</option>
            <option value = "white_theme">White Theme</option>
            <option value = "cute_theme">Cute Theme</option>
        </select>
        <p>We need your ids in different game for accessing your scores</p>
        <span>osu id : </span><input type="number" placeholder="727" v-model.number.lazy="entered_id"> COUCOU!!!!
        <button @click="updateSettings()">update</button> -->
</template>

<style lang="css" scoped>

#notifMenu {
    margin: 2vh 10% 0 10%;
    border-radius: 35px;
    height: 7vh;
    background-color: crimson;
    position: sticky;
    top:3vh;
}
</style>