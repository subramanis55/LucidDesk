   M S B A M L   `   `   `  ����D  @LucidDesk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LucidDesk	LucidDesk  5   	   M+clr-namespace:LucidDesk.UserControls.CommonLucidDesk.UserControls.Common  5   
       LucidDesk.loginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:LucidDesk     5   	   CUserControl+clr-namespace:LucidDesk.UserControls.Common     5   
   
������   � Icon$1  +/LucidDesk;Component/Resources/DeskIcon.png��5   &    � 
ResizeMode$ NoResize=�6Y    � WindowStyle$
 None=�5	       � WindowStartupLocation$ CenterScreen=�6/    � Title$ loginWindow��5
   	   $	��450��6   $	��700��6*   .��5      � 
������ .+�5   
   ��  �� CornerRadius$ 10��6      MainColorBrush#��[  6#   $��1q�6Q     SubColorBrush#��[ 6e   ._�5      � .+�5      �� .+�5      ]� $��20 0 0 0q�6!   $��50��63   $��Center=�6?   .+�5      �� $��0��6    $��20��60   $��20��6=   $��0 7 0 0q�6H   $1��+/LucidDesk;component\Resources\DeskIcon.png��6Y   6�   �� 5       �� TextAlignment$ Center=�6$   $��10 0 0 0q�6;   $��Center=�6M   $��1��6h   $
��Desk��6x   $��20(�6�   #��[  6�   6�   �� 5      -   5        $��30��5   !   $��30��6-   $��0 0 10 0q�68   $��Right=�6J   $��Right=�6a     CloseButton#��[ 6�    � ToolTip 5      _� 5   "   $��Close��6*   6<   .��5      F� $��Stretch=�6#   $��12��6?   $��12��6K   $��Stretch=�6W   
�������$	��1.5��6�   +����        � �  � �     � � �     	6�   5      5      6   0LucidDesk.UserControls.Common.ProfileImageSelect  5       -   5          Name$ ProfileImage��5    5   $	��100��6K   $	��100��6X   $��Center=�6e   $��Center=�6�   6�   3   -LucidDesk.UserControls.Common.AnimatedTextBox  5!      -   5        	  Name$	 ProfileNameTextBox��5!   2   $��45��6O   $	��350��6[   $��
25 25 25 3q�6g   "
   AnimatedTextBoxBorderRadius$
 4Q�6{      AnimatedTextBoxFontSize$ 17Q�6�       AnimatedTextBoxBackground
 �����    AnimatedTextBoxForeground
 �   �!   AnimatedTextBoxPlaceHolder$
 Name��6�   �� 5"      -   5        $��	NameError��5"      $y�Hidden=�6.   $��Enter the name��6B   $��20 3 0 0q�6[   ���#FF5739$��25��6�   $	��350��6�   $��Center=�6�   6�     5#      -   5        $	 passwordTextBox��5#   2      IsPasswordType . $��45��6c   $	��350��6o   $��	25 2 25 3q�6{   $
 4Q�6�   $ 17Q�6�   
 �����
 �   �$ Password��6  �� 5$      -   5        $��PasswordError��5$      $y�Hidden=�61   $��Enter the Password��6E   ���#FF5739$��20 3 0 0q�6w   $��25��6�   $	��350��6�   $��Center=�6�   6�   �� 5%      $��70��6   ._�5&      �� -   5        $��40��5&   !   $��18(�6.   $
��Next��6<   $	��140��6K   #��[  6W     NormalButton#��[ 6�   6�   5'      5(      5)      5*      5-      5.      