   M S B A M L   `   `   `  ����D  @LucidDesk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LucidDesk	LucidDesk  5   	       LucidDesk.ConnectWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:LucidDesk     5   	     � 
ResizeMode$  NoResize=�6/    � WindowStyle$
 None=�5         SubColorBrush#��[  6/    � Title$ ConnectWindow��5   	   $	��200��6   $	��500��6,   .��5	      �  � RowDefinitions 5
   
   �� 5      $��50 �6   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   �� $��0��6   .+�5      �� -   5        $��TitleTextBlock��5   %    �� Padding$ 10q�6   $��DemiBold"�6;   $��18(�6Q   
Connection6k   5      �� 5   
   $��1��6   .+�5      �� $��Center=�6   $��Center=�67   $ 10q�6S   $��14(�6b   
Connection6|   5      ]� 5   
   $��2��6   .+�5      �� -   5        $��Right=�5      $��Right=�6K   $��Cancel��6c     NormalButton#��[ 6t     MainColorBrush#��[ 6�   $��0 0 25 0q�6�   $	��125��6�   $��35��6�   5      �� 5      $��Right=�6   $��1q�62   
�������$��Right=�6`   
���   �
�������#��[ 6�   $��0 0 25 0q�6�   $	��125��6�   $��35��6�   .��5      �� -   5        $��AnimatedGifImage��5      $��32��60   $��32��6<   6I   5      5      5      5      