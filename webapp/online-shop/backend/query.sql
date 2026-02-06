--create database online_shop

select * from categories;

insert into panel_users values(1,'System', 'system',now(),1,now(),1);
insert into categories values(1, 1, null, 'T-shirt', 't-shirt',now(),1,now(),1);
insert into categories values(2, 1, null, 'Accessory', 'accessory',now(),1,now(),1);
insert into categories values(3, 2, 2, 'Glasses', 'glasses',now(),1,now(),1);
insert into categories values(4, 3, 3, 'UV', 'uv',now(),1,now(),1);
insert into categories values(5, 3, 3, 'Simple', 'simple',now(),1,now(),1);
        
insert into colors values(1,'Red', 'red',now(),1,now(),1);
insert into sizes values(1,'S', 's',now(),1,now(),1);
insert into brands values(1,'Pegador', 'pegador',now(),1,now(),1);
insert into products values(1, 'sample product 1', 1,'T-shirt Pegador', 't-shirt-pegador',now(),1,now(),1,1);
insert into product_variants values
        (1, 200000, 1, 2, 'sample product variant 1', 1, 'T-shirt Pegador Red', 't-shirt-pegador-red',now(),1,now(),1),
        (2, 1000.0, 2, 1, 'sample product variant 1', 1, 'T-shirt Pegador Green', 't-shirt-pegador-green',now(),1,now(),1),
        (3, 12000.0, 1, 1, 'sample product variant 1', 1, 'T-shirt Pegador Red', 't-shirt-pegador-red',now(),1,now(),1);
insert into images values
(1, 'https://lh3.googleusercontent.com/proxy/ltuIjX_efKg5bxcHbvF_0bY_U_wbDxGlGzHmmxmwC8_Pne9GVLap_LXxJllaBLyB7e9c0jLpJRd-veeTRhbn3md7nfMwafRFvX6MWmd7JfhfuvfrCojyiyXjHW5_JoAy0Q'
        , 1, 1,'Image 1', 'image-1',now(),1,now(),1),
(2, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSyu-F74hOzXJ0sIu82F0T5Rqo_0QbEN-kdCA&s'
        , 1, 1,'Image 1', 'image-1',now(),1,now(),1);
    
    
SELECT * FROM notifications;
    
    