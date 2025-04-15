<script lang="ts">

export default {
    name:"AddMap",
    data() {
        return {
            entered_id: 727,
            nanina_tag: 0,
        }
    },
    props:{
        id : {
            type : String,
            required : true,
        }
    },
    methods : {
        updateSettings(){
            this.SendToServer("add beatmap with id", JSON.stringify({id:this.entered_id.toString(), tag:this.nanina_tag}), this.id)

        }
    }
}

</script>



<template>
    <div>
        <p>Enter Osu Beatmap Id to add it to the database</p>
        <span>Beatmap Id </span><input type="number" placeholder="727" v-model.number.lazy="entered_id">
        <select v-model="nanina_tag">
            <option value = 0>Jump</option>
            <option value = 1>Streams</option>
            <option value = 2>Tech</option>
        </select>
        <button @click="updateSettings()">add</button>
    </div>
</template>

<style>
input {
    width: 120px;
}
</style>