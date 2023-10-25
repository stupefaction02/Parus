select * from [Naturistic.BL].[dbo].[BroadcastsKeywords] kb 
left join [Naturistic.BL].[dbo].[Broadcasts] br on br.Id = kb.BroadcastInfoId
left join [Naturistic.BL].[dbo].[BroadcastInfoTag] tb on tb.BroadcastsId = br.Id
left join [Naturistic.BL].[dbo].[Tags] t on t.Id = tb.TagsId
where Keyword like '%Collab?%'