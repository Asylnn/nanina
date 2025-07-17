<script lang="ts">

export default {
    name : "Homepage",

}


</script>
<template>
    <div class="flex margins">
        <h1>{{ $t(`homepage.welcome`) }}</h1>
        <p>
            {{ $t(`homepage.what_is_nanina`) }}
        </p>

        <p>
            {{ $t(`homepage.nanina_explanation`) }}
        </p>

        <p>
            {{ $t(`homepage.nanina_objective`) }}
        </p>
        <p>
             {{ $t(`homepage.wiki_link`) }}
        </p>
        <p>
            {{ $t(`homepage.nanina_supported_games`) }}
        </p>

        <p>
             {{ $t(`homepage.nanina_help.open_source`) }}
        </p>
        <ul>
            <li>
                {{ $t(`homepage.nanina_help.translation`) }}
            </li>
            <li>
                {{ $t(`homepage.nanina_help.support_games`) }}
            </li>
            <li>
                {{ $t(`homepage.nanina_help.help`) }}
            </li>
        </ul>
        
    </div>
</template>

<style lang="css" scoped>

h1{
    text-align: center;
    margin-bottom: 40px;
}

li
{
    margin-left: 50px;
    margin-bottom: 8px;
}

p{
    padding-bottom: 30px;
}

img {
    width: 100%;
    height: 100%;
}
</style>